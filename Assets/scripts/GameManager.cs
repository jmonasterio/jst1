using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.scripts.controllers;
using Toolbox;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the MOM.
/// </summary>
[RequireComponent(typeof (AudioSource))]
public class GameManager : BaseNetworkBehaviour
{
    protected GameManager()
    {
    }



    private GameObject _root;

    private static GameManager _instance;

    private static object _lock = new object();

    public static GameManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (GameManager)FindObjectOfType(typeof(GameManager));
                    //DontDestroyOnLoad(_instance);

                    //if (FindObjectsOfType(typeof (T)).Length > 1)
                    //{
                    //    Debug.LogError("[Singleton] Something went really wrong " +
                    //                  " - there should never be more than 1 singleton!" +
                    //                  " Reopenning the scene might fix it.");
                    //    return _instance;
                }

                if (_instance == null)
                {
                    //GameObject singleton = new GameObject();
                    //_instance = singleton.AddComponent<T>();
                    //singleton.name = "(singleton) " + typeof(T).ToString();


                    //Debug.Log("[Singleton] An instance of " + typeof(T) +
                    //    " is needed in the scene, so '" + singleton +
                    //    "' was created with DontDestroyOnLoad.");
                    Debug.LogError("[Singleton] Did not find game object.");
                }
                else
                {
                    // This gets set by Singleton.
                    //Debug.Log("[Singleton] Using instance already created: " +
                     //         _instance.gameObject.name);
                }


                return _instance;
            }
        }
    }


    // Handy accessors.
    public GameObject SceneRoot
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("/root");
            }
            return _root;
        }
    }

    [HideInInspector]
    public SceneController SceneController;
    [HideInInspector]
    public OfflineSceneController OfflineSceneController;
    [HideInInspector]
    public PlayController PlayController;


    // Use this for initialization
    void Awake()
    {
        // This gets set by Singleton.
        //DontDestroyOnLoad(this.gameObject);

        SceneController = GetComponent<SceneController>();
        //SceneController.Start();
        OfflineSceneController = GetComponent<OfflineSceneController>();
        //OfflineSceneController.Start();
        PlayController = GetComponent<PlayController>();
        //PlayController.Start();
    }

    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {

    }


}
