using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

/// <summary>
/// Customize this class here.
/// </summary>
public class UnityNetworkManager : NetworkManager {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static UnityNetworkManager Instance
    {
        get { return singleton.GetComponentInParent<UnityNetworkManager>(); }

    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
    }

    public override void OnMatchCreate(CreateMatchResponse matchInfo)
    {
        base.OnMatchCreate(matchInfo);
    }
}


