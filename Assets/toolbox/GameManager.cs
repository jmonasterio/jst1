using System;
using UnityEngine;
using System.Collections;
using Toolbox;


[RequireComponent(typeof(AudioSource))] 
public class GameManager : Base2DBehaviour 
{
    public struct Buttons
    {
        public const string HORIZ = "Horizontal";
        public const string FLAP = "Vertical";
    }

    public int Score = 0;
    public int Lives = 0;
    public MonoBehaviour SceneControllerPrefab;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof (GameManager)) as GameManager;
            }
            return _instance;
        }
    }

    public enum States
    {
        Playing = 0,
        Over = 1
    }
    public States State = States.Over;

    [HideInInspector]
    public MonoBehaviour SceneController;
    [HideInInspector]
    public Transform SceneRoot;

    // Use this for initialization
    void Awake()
	{
        DontDestroyOnLoad(this); // Keep running across different scenes.
        _instance = this; // Simple singleton.
        SceneRoot = this.transform.parent;
	    this.transform.parent = SceneRoot;
	    SceneController = Instantiate(SceneControllerPrefab); 
	    SceneController.transform.parent = this.transform.parent;
	}

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


    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        Lives = 4;
        Score = 0;
        State = States.Playing;
    }
}
