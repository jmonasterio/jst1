using UnityEngine;
using System.Collections;
using Assets.scripts;

public class CameraController : MonoBehaviour
{
    public Camera MainCamera;
    public float LeftMax;
    public float RightMax;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var localPlayer = SafeGameManager.LocalPlayer;
        if ( localPlayer != null)
	    {
	        var p = localPlayer.transform.position;
	        var c = MainCamera.transform.position;
	        if (p.x < LeftMax)
	        {
                MainCamera.transform.position = new Vector3(LeftMax, c.y, c.z);
            }
            else if (p.x > RightMax)
            {
                MainCamera.transform.position = new Vector3(RightMax, c.y, c.z);
            }
            else
            {
                MainCamera.transform.position = new Vector3(p.x, c.y, c.z);
            }
	}
	}
}
