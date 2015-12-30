using System;
using UnityEngine;
using System.Collections;
using Assets.scripts;
using Toolbox;

public class Player : BaseNetworkBehaviour
{

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

            bird.transform.position = SafeGameManager.SceneController.GetRandomSpawnPoint();
            

            SafeGameManager.PlayClip(bird.SpawnSound);

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


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
