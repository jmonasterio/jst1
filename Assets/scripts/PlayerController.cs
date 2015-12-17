using UnityEngine;
using System.Collections;
using Toolbox;

public class PlayerController : BaseBehaviour
{

    public struct Buttons
    {
        public const string HORIZ = "Horizontal";
        public const string FLAP = "Vertical";
    }

    public int Score = 0;
    public int Lives = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        Lives = 4;
        Score = 0;
    }

}
