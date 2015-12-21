using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts;
using Toolbox;

[RequireComponent(typeof(AudioSource))]
public class Enemy : Base2DBehaviour
{
    public enum Sizes
    {
        Big,
        Small
    }

    public enum States
    {
        Live,
        Killed
    }

#if OLD_WAY
    public struct GoNames
    {
        public const string ASTEROID_BULLET_CONTAINER = "AlienBulletContainer";
    }
#endif

    public Sizes Size;
    public AudioClip ExplosionSound;
    public ParticleSystem ExplosionParticlePrefab;
    public AudioClip ShootSound;

    private ParticleSystem _explosionParticleSystem;


    private List<Vector3> _path = new List<Vector3>();
    private int _curPoint = 0;

    public void SetPath(List<Vector3> newPath)
    {
        _path = newPath;
        this.transform.position = _path[0];
    }

    // Use this for initialization
    void Start()
    {
        //_explosionParticleSystem = ExplosionParticlePrefab.InstantiateAtTransform( this.transform);
        //_explosionParticleSystem.loop = false;
        //_explosionParticleSystem.Stop();
    }


    // Update is called once per frame
    void FixedUpdate ()
    {
#if OLD_WAY
        var rigidBody = GetComponent<Rigidbody2D>();

        var target = _path[_curPoint];
        var curPos = this.transform.position;

        if (Vector2.Distance(target, curPos)<= 0.25)
        {
            GoToNextPoint();
        }
        else
        {
            // Go towards
            var dir = target - curPos;
            rigidBody.velocity = dir.normalized * travelSpeed*Time.deltaTime;

        }

        var hit = Physics2D.Raycast(transform.position, rigidBody.velocity, distance:10.0f, layerMask:9 /* Asteroid */);
        if (hit.collider != null)
        {
            float distance = Vector2.Distance(hit.point, transform.position); //n + Vector2.Up From2D(rigidBody.velocity.normalized*transform.localScale.magnitude)); // Extra math to not hit self.
            if (distance > 0 && distance < 4.0f)
            {
                print("distance: " + distance);
                distance = 0;
            }
            
        }
#endif

    }


    private void EnemyKilled()
    {
        this.gameObject.Show(false);
        _explosionParticleSystem.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        SafeGameManager.SceneController.DestroyEnemy(this, explode: true);
        SafeGameManager.PlayClip(ExplosionSound);
        SafeGameManager.PlayController.Score += ((this.Size == Enemy.Sizes.Small) ? 1000 : 500);
        Destroy(this.gameObject, _explosionParticleSystem.duration + 0.5f);
    }

    private void GoToNextPoint()
    {
        _curPoint++;
        if (_curPoint >= _path.Count)
        {
            //If end of path, we're done.
            SafeGameManager.SceneController.DestroyEnemy(this, explode: false);
        }

    }


    
}
