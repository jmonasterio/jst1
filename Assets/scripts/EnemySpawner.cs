using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Assets.scripts
{
    public class EnemySpawner : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
            SafeGameManager.SceneController.OnStartServer();
        }

        public override void OnStartLocalPlayer() // this is our player
        {
            base.OnStartLocalPlayer();
        }
    }
}
