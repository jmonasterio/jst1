using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.scripts.controllers;
using Toolbox;
using UnityEngine.Networking;

/// <summary>
/// This is the MOM.
/// </summary>
[RequireComponent(typeof (AudioSource))]
public class GameManager : Singleton<GameManager>
{
    protected GameManager()
    {
    }

    // Handy accessors.
    public GameObject SceneRoot;
    [HideInInspector]
    public SceneController SceneController;
    [HideInInspector]
    public OfflineSceneController OfflineSceneController;
    [HideInInspector]
    public PlayController PlayController;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        SceneController = GetComponent<SceneController>(); 
        OfflineSceneController = GetComponent<OfflineSceneController>();
        PlayController = GetComponent<PlayController>();
    }

   

    // Update is called once per frame
    void Update()
    {

    }


}
