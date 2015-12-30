using System;
using UnityEngine;
using System.Collections;
using Assets.scripts;
using Toolbox;

public class Player : BaseNetworkBehaviour
{
    private bool _flapButtonDown;

    public override void OnStartLocalPlayer() // this is our player
    {
        base.OnStartLocalPlayer();
        try
        {
            /// NOTE: GameManager and UnityNetworkManager not safe here! Because this is before sceneController created, etc.
            /// 

            // change color of local player.
            var bird = GetComponent<Bird>();
            bird.SetAsLocalPlayer();

            Respawn();

            // TBD _birdPlayer.GetComponent<Rigidbody2D>().gravityScale = 0.0f; // Turn off gravity.
            //this.transform.parent = SafeGameManager.SceneRoot;
            // this.gameObject.SetActive(true);


#if OLD_WAY
    // Can't do this here when switching scenes. Too early.
        

        if (isServer)
        {
            var x = SafeGameManager.SceneController.EnemyPrefab.InstantiateInTransform(SafeGameManager.SceneRoot);
            NetworkServer.Spawn(x.gameObject);
        }
#endif
        }
        catch (Exception ex)
        {
            Debug.Log("BIRD.CS: " + ex.Message);
        }


    }

    public void Respawn()
    {
        var bird = GetComponent<Bird>();
        bird.transform.position = SafeGameManager.SceneController.GetRandomSpawnPoint();
        SafeGameManager.PlayClip(bird.SpawnSound);
    }


    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        _flapButtonDown = Input.GetButtonDown(global::PlayController.Buttons.FLAP);


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Network.isServer)
        {
            return;
        }
        if (!isLocalPlayer)
        {
            // I think remote player will get animated because of NetworkAnimation component.
            return;
        }

        try
        {

            // Get user inputs
            float horz = Input.GetAxisRaw(PlayController.Buttons.HORIZ);
            bool vert = _flapButtonDown;
            _flapButtonDown = false;

            var bird = GetComponent<Bird>();
            bird.ApplyInputsForMovement(horz, vert);
            bird.AnimateBird();

        }
        catch (Exception ex)
        {
            Debug.Log("Player.FixedUpdate: " + ex.Message);
        }
    }

}
