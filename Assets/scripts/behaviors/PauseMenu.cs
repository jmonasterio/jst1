using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.scripts
{
    public class PauseMenu : BaseNetworkBehaviour
    {
        public Rect windowRect = new Rect(295, 175, 0, 0);
        public bool gamePaused = false;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isServer)
                {
                    NetworkManager.singleton.StopHost();
                }
                else
                {
                    NetworkManager.singleton.client.Disconnect();
                    SceneManager.LoadScene("offline");
                }
            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                if (gamePaused)
                {
                    gamePaused = false;
                    Time.timeScale = 1.0f;
                }
                else
                {
                    gamePaused = true;
                    Time.timeScale = 0.0f;
                }
            }
        }
        void OnGUI()
        {
            if (gamePaused)
            {
                Time.timeScale = 0.0f;
                GUILayout.Window(0, windowRect, Pause,
                    "Game Paused", GUILayout.Width(100));
                //SafeGameManager.NetController.GetComponent<NetworkManagerHUD>().showGUI = true;
            }
        }
        void Pause(int windowPause)
        {
            if (GUILayout.Button("Resume"))
            {
                Time.timeScale = 1.0f;
                gamePaused = false;
                //SafeGameManager.NetController.GetComponent<NetworkManagerHUD>().showGUI = false;
            }
        }
    }
}
