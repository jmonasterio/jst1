using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts;
using Toolbox;
using Random = UnityEngine.Random;

public class Enemy : BaseNetworkBehaviour
{
    public enum AiTactics
    {
        Follow,
        RunAway,
    }

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

    public AiTactics AiTactic;

#if OLD_WAY
    public struct GoNames
    {
        public const string ASTEROID_BULLET_CONTAINER = "AlienBulletContainer";
    }
#endif

    //public Sizes Size;
    //public AudioClip ExplosionSound;
    //public ParticleSystem ExplosionParticlePrefab;
    //public AudioClip ShootSound;

    //private ParticleSystem _explosionParticleSystem;


    //private List<Vector3> _path = new List<Vector3>();
    ///private int _curPoint = 0;

#if OLD_WAY
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
#endif

    void Start()
    {
        AiTactic = (Random.Range(0, 2) == 0) ? AiTactics.Follow : AiTactics.RunAway;
    }


    void FixedUpdate()
    {
        if (!Network.isServer)
        {
            var enemyBird = GetComponent<Bird>();
            float horz = 0.0f;
            float vert = 0.0f; 

            var followPlayer = SafeGameManager.SceneController.Players.FirstOrDefault();
            if (followPlayer != null)
            {
                this.CalcMove(followPlayer, out horz, out vert);

            }


            if ( (followPlayer == null) || (followPlayer.IsDead) ) // TBD Need better hack here.
            {
                // Player is unspawned/dead.
            }
            else
            {
                enemyBird.ApplyInputsForMovement(horz, vert);
            }

            enemyBird.AnimateBird();
        }
    }

    private void CalcMove(Player followPlayer, out float horz, out float vert)
    {
        float distance = Vector3.Distance(this.transform.position, followPlayer.transform.position);

        vert = 0;
        horz = 0;
        if (AiTactic == AiTactics.Follow)
        {
            horz = -Mathf.Sign(this.transform.position.x - followPlayer.transform.position.x)*0.25f;
            if (this.transform.position.y < followPlayer.transform.position.y)
            {
                vert = 0.3f; // Represents soft flap
            }
        }
        else if (AiTactic == AiTactics.RunAway)
        {
            if (distance < 2.0f)
            {
                horz = +Mathf.Sign(this.transform.position.x - followPlayer.transform.position.x)*0.1f;
            }
            if (this.transform.position.y > followPlayer.transform.position.y)
            {
                vert = 0.3f; // Represents soft flap
            }
            if (this.transform.position.y > 1.0f)
            {
                vert = 0.0f;
            }
            else if (this.transform.position.y < -1.0f)
            {
                vert = 0.3f; // Hard flap
            }
        }
        else
        {
            throw new NotImplementedException("Unknown AiTactic.");
        }


        // Just try to make sure player stays out of lava.
        if (this.transform.position.y < -1.3f)
        {
            vert = 1.0f; // hard falp
        }

    }




#if OLD_WAY
    // Update is called once per frame
    void FixedUpdate ()
    {

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

#endif


}
