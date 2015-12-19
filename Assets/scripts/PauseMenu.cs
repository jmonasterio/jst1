using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;

namespace Assets.scripts
{
    public class PauseMenu : BaseBehaviour
    {
        public Rect windowRect = new Rect(295, 175, 0, 0);
        public bool gamePaused = false;
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
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
            }
        }
        void Pause(int windowPause)
        {
            if (GUILayout.Button("Resume"))
            {
                Time.timeScale = 1.0f;
                gamePaused = false;
            }
        }
    }
}
