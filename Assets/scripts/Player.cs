﻿using System.Collections;
using System.Collections.Generic;
using Assets.scripts;
using UnityEngine;
using Toolbox;

[RequireComponent(typeof(Blinker))]
public class Player : Base2DBehaviour
{
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

    public float Thrust = 500.0f;
    public float MaxSpeed = 25.0f;
    public int PlayerIndex = 0; // Or 1, for 2 players.
    public Bird BirdPrefab;
    public Rider RiderPrefab;

    public AudioClip ExplosionSound;
    public AudioClip FlapSound;

    public ParticleSystem ExplosionParticlePrefab;

    private ParticleSystem _explosionParticleSystem;

    private enum State
    {
        Alive = 0,
        Killed = 1
    }


    private State _state;
    private GameObject _bulletsContainer;
    private float _lastHyperSpaceTime;
    private float _lastFlap;


    // Use this for initialization
    public void Start()
    {
        //_bulletsContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);

        //_explosionParticleSystem = ExplosionParticlePrefab.InstantiateAtTransform( this.transform);
        //_explosionParticleSystem.loop = false;
        //_explosionParticleSystem.Stop();

        _state = State.Alive;
    }

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
        GameManager.Instance.PlayClip(ExplosionSound);
        Destroy(this.gameObject, _explosionParticleSystem.duration + 0.5f);
    }


    // Update is called once per frame
    void Update ()
    {


        if (_state != State.Killed)
        {
            var animator = this.transform.GetComponent<Animator>();
            animator.enabled = true;
            

            //base.DebugForceSinusoidalFrameRate();

            var rigidBody = GetComponent<Rigidbody2D>();

            float horz = Input.GetAxisRaw(GameManager.Buttons.HORIZ);
            if (horz != 0.0f)
            {
                rigidBody.AddRelativeForce(Vector2.right*200.0f*Time.deltaTime*horz, ForceMode2D.Force);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
                this.transform.localScale = new Vector3(Mathf.Sign(horz), 1, 1);
            }


            bool vert = Input.GetButtonDown(GameManager.Buttons.FLAP);

            // Maybe a thruster component? Or maybe Rotator+Thruster=PlayerMover component.
            if (vert)
            {
                rigidBody.AddForce(Vector2.up*Thrust*Time.deltaTime, ForceMode2D.Impulse);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
                GameManager.Instance.PlayClip(FlapSound);

                animator.SetBool(AnimParams.InFlap, true);
                _lastFlap = Time.time;
            

                //var sprite = this.GetComponent<SpriteRenderer>();
                //sprite.enabled = false;

                //var anim = this.GetComponent<Animation>();
                //anim.enabled = true;
                //if (!anim.isPlaying)
                //{
                //    anim.wrapMode = WrapMode.Once;
                //    anim.Play();
                //}
                //else
                //{
                //    anim.Stop();
                //}
            }
            else
            {
                if (Time.time - _lastFlap > 0.5f)
                {
                    animator.SetBool(AnimParams.InFlap, false);
                    _lastFlap = 0.0f;
                }
            }
        }
        else
        {

        }

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
        var abc = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);
        GameObjectExt.DestroyChildren(abc);

    }
}
