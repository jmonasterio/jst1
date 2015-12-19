using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;

namespace Assets.scripts
{
    public class SafeGameManager
    {
        public static SceneController SceneController
        {
            get { return (GameManager.Instance.SceneController) as SceneController; }
        }

        public static NetworkController NetworkController
        {
            get { return (GameManager.Instance.NetworkControllerPrefab) as NetworkController; }
        }

        public static PlayController PlayController
        {
            get { return (GameManager.Instance.PlayerController) as PlayController; }
        }


        public static void PlayClip(AudioClip clip)
        {
            GameManager.Instance.PlayClip(clip);
        }

        public static void SetGameState(GameManager.States state)
        {
            GameManager.Instance.State = state;
        }

        public static GameManager.States GetGameState()
        {
            return GameManager.Instance.State;
        }

    }
}
