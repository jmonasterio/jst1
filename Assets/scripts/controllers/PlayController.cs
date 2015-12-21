using UnityEngine;
using System.Collections;
using Toolbox;

public class PlayController : Base2DBehaviour
{
    // TBD: Maybe in a playController
    public enum States
    {
        Playing = 0,
        Over = 1
    }

    public States State = States.Over;


    public struct Buttons
    {
        public const string HORIZ = "Horizontal";
        public const string FLAP = "Vertical";
    }

    public int Score = 0;
    public int Lives = 0;

    // Use this for initialization
    public void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        State = States.Playing;
        Lives = 4;
        Score = 0;
    }

    // TBD: Put this in a soundController component attached to GameManager.
    // Safer to play sounds on the game object, since bullets or or asteroids may get destroyed while sound is playing???
    public void PlayClip(AudioClip clip, bool loop = false)
    {
        if (State == States.Over)
        {
            // No sound when not playing.
            return;
        }
        var src = GetComponent<AudioSource>();
        src.loop = loop;
        src.PlayOneShot(clip, 1.0f);
    }


}
