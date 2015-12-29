using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.scripts
{
    public class TitleUI : MonoBehaviour
    {

        public Texture TitleTexture;

        public string RiderName;
        private string _lastError;

        void OnGUI()
        {

            try
            {
                int posY = Screen.height / 2 - 100;
                GUI.DrawTexture(new Rect(
                    Screen.width/2 - TitleTexture.width,
                    Screen.height/2 - TitleTexture.height*2 - 150,
                    TitleTexture.width*2,
                    TitleTexture.height*2), TitleTexture);


                GUI.Label(new Rect(Screen.width/2 - 90, posY, 80, 20), "Rider Name:");
                RiderName = GUI.TextField(new Rect(Screen.width/2 - 10, posY, 100, 20), RiderName, 20);
                posY += 40;

                if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Single Player Game\n[Press S]") ||
                    Input.GetKeyDown(KeyCode.S))
                {
                    _lastError = "";
                    NetworkManager.singleton.StartHost();
                }
                posY += 38;

                if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Host Multiplayer Game\n[Press H]") ||
                    Input.GetKeyDown(KeyCode.H))
                {
                    _lastError = "";
                    NetworkManager.singleton.StartMatchMaker();
                    SceneManager.LoadScene(SceneController.Scenes.Host);
                }
                posY += 38;

                if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Join Multiplayer Game\n[Press J]") ||
                    Input.GetKeyDown(KeyCode.J))
                {
                    _lastError = "";
                    NetworkManager.singleton.StartMatchMaker();
                    SceneManager.LoadScene(SceneController.Scenes.Host);
                }
                posY += 38;

                if (GUI.Button(new Rect(Screen.width/2 - 100, posY, 200, 33), "Join Local Game\n[Press K]") ||
                    Input.GetKeyDown(KeyCode.K))
                {
                    _lastError = "";
                    NetworkManager.singleton.StartClient();
                }
                posY += 44;
                GUI.Label(new Rect(Screen.width / 2 - 100, posY, 200, 33), _lastError);
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
            }
        }
    }

}
