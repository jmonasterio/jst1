﻿using UnityEngine;
using System.Collections;
using Assets.scripts;
using Toolbox;

public class Score : Base2DBehaviour
{

    private int _score;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var curScore = SafeGameManager.PlayController.Score;
        if (_score != curScore )
        {
            GetComponent<TextMesh>().text = "" + curScore;
            _score = curScore;
        }
	}
}
