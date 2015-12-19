using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.controllers;
using Toolbox;
using UnityEngine;

namespace Assets.scripts
{
    public class SafeGameManager
    {
        public static SceneController SceneController
        {
            get { return (GameManager.SceneController) as SceneController; }
        }

        public static OfflineSceneController OfflineSceneController
        {
            get { return (GameManager.OfflineSceneController) as OfflineSceneController; }
        }

        public static NetworkController NetworkController
        {
            get { return (GameManager.NetworkController) as NetworkController; }
        }

        public static PlayController PlayController
        {
            get { return (GameManager.PlayerController) as PlayController; }
        }

        public static void StartGame()
        {
            GameManager.__instance.StartGame();
        }

        public static Transform SceneRoot
        {
            get { return GameManager.__instance.SceneRoot.transform; } 
        }

        public static void PlayClip(AudioClip clip)
        {
            GameManager.__instance.PlayClip(clip);
        }

        public static void SetGameState(GameManager.States state)
        {
            GameManager.State = state;
        }

        public static GameManager.States GetGameState()
        {
            return GameManager.State;
        }

    }
}
