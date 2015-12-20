using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.controllers;
using Toolbox;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts
{
    public static class SafeGameManager
    {
        public static SceneController SceneController
        {
            get { return (GameManager.Instance.SceneController); }
        }

        public static OfflineSceneController OfflineSceneController
        {
            get { return (GameManager.Instance.OfflineSceneController) as OfflineSceneController; }
        }

        public static PlayController PlayController
        {
            get { return (GameManager.Instance.PlayController); }
        }

        public static void StartGame()
        {
            GameManager.Instance.PlayController.StartGame();
        }

        public static Transform SceneRoot
        {
            get { return GameManager.Instance.SceneRoot.transform; } 
        }

        public static void PlayClip(AudioClip clip)
        {
            GameManager.Instance.PlayController.PlayClip(clip);
        }

        public static void SetGameState(PlayController.States state)
        {
            GameManager.Instance.PlayController.State = state;
        }

        public static PlayController.States GetGameState()
        {
            return GameManager.Instance.PlayController.State;
        }

    }
}
