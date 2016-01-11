using System;
using UnityEngine;
using System.Collections;
using Assets.scripts;
using Toolbox;


public class Egg : MonoBehaviour
{
    public event TypedEventHandler.EventHandler<Egg> Hatched;
    //public event EventHandler Hatched;

    public enum Mothers
    {
        Player,
        Enemy1,
        Enemy2,
        Enemy3
    }
    public Mothers Mother;
    public float HatchTime;
    public float BlinkStartTime;
    private Blinker _blinker;

	// Use this for initialization
	void Start ()
	{
	    HatchTime = Time.time + 3.0f;
	    BlinkStartTime = Time.time + 2.0f;
	}
	
	// Update is called once per frame
	void Update () {

        if ( Time.time > BlinkStartTime && (_blinker == null))
	    {
	        _blinker = GetComponent<Blinker>();
            _blinker.Blink();
        }
        else if( Time.time > HatchTime )
        {
            HatchNow();
        }
	}

    private void HatchNow()
    {
        switch (Mother)
        {
            default:
                if (this.Hatched != null)
                {
                    this.Hatched(this, new EventArgs());
                }

                //SafeGameManager.SceneController.HatchEgg(this);
                break;
        }
    }
}
