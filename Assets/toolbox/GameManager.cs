using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Toolbox;

/// <summary>
/// This is the MOM.
/// </summary>
[RequireComponent(typeof(AudioSource))] 
public class GameManager : BaseBehaviour 
{
    public MonoBehaviour SceneControllerPrefab;
    public MonoBehaviour OfflineSceneControllerPrefab;
    public MonoBehaviour NetworkControllerPrefab;
    public MonoBehaviour PlayerControllerPrefab;

    // TBD: Maybe in a gameController
    public enum States
    {
        Playing = 0,
        Over = 1
    }
    public static States State = States.Over;


    [HideInInspector]
    public GameObject SceneRoot;

    [HideInInspector]
    public static MonoBehaviour SceneController;
    [HideInInspector]
    public static MonoBehaviour OfflineSceneController;
    [HideInInspector]
    public static MonoBehaviour NetworkController;
    [HideInInspector]
    public static MonoBehaviour PlayerController;

    public static GameManager __instance;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this); // Keep running across different scenes.

        //Check if instance already exists
        if (__instance == null)
        {

            //if not, set instance to this
            __instance = this;
        }

        //If instance already exists and it's not this:
        else if (__instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

        SceneRoot = GameObjectExt.SceneRoots().First( _ => _.name == "root");
        this.transform.parent = SceneRoot.transform;

        SceneController = InstantiateController(SceneControllerPrefab);
        OfflineSceneController = InstantiateController(SceneControllerPrefab);
        NetworkController = InstantiateController(NetworkControllerPrefab);
        PlayerController = InstantiateController(PlayerControllerPrefab);
    }

    private MonoBehaviour InstantiateController(MonoBehaviour prefab)
    {
        MonoBehaviour ret = Instantiate(prefab);
        ret.transform.parent =  SceneRoot.transform;
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
