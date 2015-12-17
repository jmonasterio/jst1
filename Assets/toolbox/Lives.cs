﻿using System;
using UnityEngine;
using System.Collections;
using Assets.scripts;
using Toolbox;

namespace Assets.Scripts
{
    public class Lives : Base2DBehaviour
    {

        private int _lives = -1;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var lives = SafeGameManager.PlayerController.Lives;
            if (_lives != lives)
            {
                if (lives > 0)
                {
                    GetComponent<TextMesh>().text = new String('^', lives);
                }
                else
                {
                    GetComponent<TextMesh>().text = string.Empty;
                }
                _lives = lives;
                GetComponent<Blinker>().BlinkText(2.0f, 0.1f, Color.gray);
            }
        }
    }
}
