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
            get { return (GameManager.Instance.SceneController as Base2DBehaviour) as SceneController; }
        }

        public static void PlayClip(AudioClip clip)
        {
            GameManager.Instance.PlayClip(clip);
        }

    }
}
