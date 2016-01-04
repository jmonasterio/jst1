using System;
using UnityEngine;
using System.Collections;
using Assets.scripts;
using Assets.toolbox;
using Toolbox;

public class Player : BaseNetworkBehaviour
{
    private bool _flapButtonDown;

    public bool IsDead;

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

            //SafeGameManager.SceneController.RespawnPlayer(this);

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

    public void RespawnAt( Vector3 pos)
    {
        var bird = GetComponent<Bird>();

        IsDead = false;

        var teleportable = this.GetComponent<Teleportable>();
        teleportable.StartTeleportTo(pos);

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
            float vert = _flapButtonDown ? 1.0f : 0.0f;
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
