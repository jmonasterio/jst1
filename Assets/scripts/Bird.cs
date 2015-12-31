using System;
using Assets.scripts;
using Assets.scripts.behaviors;
using UnityEngine;
using Toolbox;
using UnityEngine.Networking;
using PlayerController = UnityEngine.Networking.PlayerController;

[RequireComponent(typeof (Blinker))]
public class Bird : BaseNetworkBehaviour
{
    public Sprite LocalPlayerSprite;

    public class GoNames
    {
        public const string BULLET_CONTAINER_NAME = "PlayerBulletsContainer";
    }

    public struct AnimParams
    {
        public const string InFlap = "InFlap"; // Bool
        public const string InBrake = "InBrake"; // Bool
        public const string Grounded = "Grounded"; // Bool
        public const string HorzSpeed = "HorzSpeed"; // Float
    }

    public bool MovementSoundsOn;
    public Legs LegsChild;
    public Rider RiderChild;
    public HealthBar HealthBarChild;

    [SyncVar] public float FaceDir = -1.0f;


    public bool InBrake = false;
    public bool InFlap = false;

    public float Thrust = 500.0f;
    public float SideThrust = 100.0f;
    public float BrakingSpeed = 1.0f;
    public float MaxSpeed = 25.0f;

    public AudioClip ExplosionSound;
    public AudioClip FlapSound;
    public AudioClip BrakeSound;
    public AudioClip SpawnSound;

    public ParticleSystem ExplosionParticlePrefab;

    private ParticleSystem _explosionParticleSystem;

    private enum State
    {
        Alive = 0,
        Killed = 1
    }

    public override void OnStartClient()
    {
        print(this.SpawnSound);
        System.Diagnostics.Debug.Assert(isClient);

        SafeGameManager.PlayClip(this.SpawnSound);
        this.GetComponent<Blinker>().BlinkSpriteAlpha(this.SpawnSound.length, 0.05f);
    }

    public override void OnStartServer()
    {
        // Sometimes, local player is running on the server, and we need to make sure we hear their spawn sound.
        if (isLocalPlayer)
        {
            OnStartClient(); // hack.
        }
    }


    private float _lastFlap;
    private Animator _animator;
    private Rigidbody2D _rigidBody;


    // Use this for initialization
    public void Start()
    {
        //_bulletsContainer = SafeGameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);

        //_explosionParticleSystem = ExplosionParticlePrefab.InstantiateAtTransform( this.transform);
        //_explosionParticleSystem.loop = false;
        //_explosionParticleSystem.Stop();

        //this.GetComponent<SpriteRenderer>().enabled = false; // Only using animations at runtime.
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();

    }

#if OLD_WAY
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_state == State.Killed)
        {
            return;
        }
        if (other.GetInstanceID() == this.GetInstanceID())
        {
            // Avoid collision with self.
            return;
        }
        if (other.gameObject.GetComponent<Bullet>())
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.Source == Bullet.Sources.PlayerShooter)
            {
                // We don't allow shooting ourself
                return; // 
            }
        }
        PlayerKilled();
    }

    /// <summary>
    /// Could also be called by alien bullet.
    /// </summary>
    private void PlayerKilled()
    {
        _state = State.Killed;
        Show(false);
        _explosionParticleSystem.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        SafeGameManager.SceneController.PlayerKilled(this);
        SafeGameManager.PlayClip(ExplosionSound);
        Destroy(this.gameObject, _explosionParticleSystem.duration + 0.5f);
    }
#endif



    public void AnimateBird()
    {
        // Only works when animator is enabled. The faceDir was not handled by the NetworkTransform.
        this.transform.localScale = new Vector3(FaceDir, 1, 1);

        // Using networkAnimator, so assume this stuff will get sent over.
        var horzSpeed = Mathf.Abs(_rigidBody.velocity.x);
        _animator.SetFloat(AnimParams.HorzSpeed, horzSpeed);
        _animator.SetBool(Bird.AnimParams.Grounded, LegsChild.IsGrounded);
        _animator.SetFloat(Bird.AnimParams.HorzSpeed, Mathf.Abs(_rigidBody.velocity.x));
        _animator.SetBool(AnimParams.InBrake, InBrake);
        _animator.SetBool(AnimParams.InFlap, InFlap);
    }

    // Can be reused for AI of enemies.
    public void ApplyInputsForMovement(float horz, float vert)
    {
        if (vert < 0.0f)
        {
            throw new Exception( "Unsupported. Vert should be between 0-1");
        }

        if (horz != 0.0f)
        {
            _rigidBody.AddForce(Vector2.right * SideThrust * horz, ForceMode2D.Force);
            _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, MaxSpeed);

            // Sync var will get sent over network so other player flips.
            var newFaceDir = Mathf.Sign(horz);
            if (newFaceDir != FaceDir)
            {
                FaceDir = newFaceDir;
                if (!Network.isServer)
                {
                    CmdSetFaceDir(newFaceDir);
                }
            }
            InBrake = false;
        }
        else
        {
            var horzSpeedLocal = Mathf.Abs(_rigidBody.velocity.x);
            bool wasInBrake = InBrake;
            InBrake = (horzSpeedLocal > BrakingSpeed) && (horz == 0.0f) && LegsChild.IsGrounded;
            if (MovementSoundsOn)
            {
                if (InBrake && !wasInBrake)
                {
                    SafeGameManager.PlayClip(BrakeSound);
                }
            }
        }


        // Maybe a thruster component? Or maybe Rotator+Thruster=PlayerMover component.
        if (vert > 0.0f)
        {
            _rigidBody.AddForce(Vector2.up * Thrust * vert, ForceMode2D.Impulse);
            _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, MaxSpeed);
            if (MovementSoundsOn)
            {
                SafeGameManager.PlayClip(FlapSound);
            }

            InFlap = true;
            _lastFlap = Time.time;
        }
        else
        {
            if (Time.time - _lastFlap > 0.5f)
            {
                InFlap = false;
                _lastFlap = 0.0f;
            }
        }

    }

    [Command]
    public void CmdSetFaceDir(float newFaceDir)
    {
        FaceDir = newFaceDir;
    }


    public void Show(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }

    void OnDestroy()
    {
        // Cleanup
        if (_explosionParticleSystem != null)
        {
            _explosionParticleSystem.Stop();
        }
        GameObjectExt.SafeDestroy(ref _explosionParticleSystem);
    }

    public static void ClearBullets()
    {
        var abc = SafeGameManager.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);
        GameObjectExt.DestroyChildren(abc);

    }

    public void SetAsLocalPlayer()
    {
        this.RiderChild.GetComponent<SpriteRenderer>().sprite = this.LocalPlayerSprite;
    }

}
