using UnityEngine.Networking;

/// <summary>
/// If we need to add any special code for the builtin network manager.
/// </summary>
public class UnityNetworkManager : UnityEngine.Networking.NetworkManager 
{


    public static UnityNetworkManager Instance
    {
        get { return singleton.GetComponentInParent<UnityNetworkManager>(); }

    }

   
}


