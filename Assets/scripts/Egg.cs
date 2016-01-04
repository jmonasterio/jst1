using UnityEngine;
using System.Collections;
using Assets.scripts;

public class Egg : MonoBehaviour
{
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
                var pos = this.transform.position;
                pos = new Vector3(pos.x, pos.y+0.1f, pos.z); // Spawn player a little higher so feet not in ground.
                SafeGameManager.SceneController.SpawnEnemyAt(pos);
                Destroy(this.gameObject);
                break;
        }
    }
}
