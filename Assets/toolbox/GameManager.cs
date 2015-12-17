using System;
using UnityEngine;
using System.Collections;
using Toolbox;

/// <summary>
/// This is the MOM.
/// </summary>
[RequireComponent(typeof(AudioSource))] 
public class GameManager : BaseBehaviour 
{
    public MonoBehaviour SceneControllerPrefab;
    public MonoBehaviour NetworkControllerPrefab;
    public MonoBehaviour PlayerControllerPrefab;

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

    // TBD: Maybe in a gameController
    public enum States
    {
        Playing = 0,
        Over = 1
    }
    public States State = States.Over;

    [HideInInspector]
    public Transform SceneRoot;
    [HideInInspector]
    public MonoBehaviour SceneController;
    [HideInInspector]
    public MonoBehaviour NetworkController;
    [HideInInspector]
    public MonoBehaviour PlayerController;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this); // Keep running across different scenes.

        _instance = this; // Simple singleton.

        SceneRoot = this.transform.parent;
        this.transform.parent = SceneRoot;

        SceneController = InstantiateController(SceneControllerPrefab);
        NetworkController = InstantiateController(NetworkControllerPrefab);
        PlayerController = InstantiateController(PlayerControllerPrefab);
    }

    private MonoBehaviour InstantiateController(MonoBehaviour prefab)
    {
        MonoBehaviour ret = Instantiate(prefab);
        ret.transform.parent =  SceneRoot;
        return ret;
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
        State = States.Playing;
    }

}
