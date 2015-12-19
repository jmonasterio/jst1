using UnityEngine;
using System.Collections;
using Toolbox;
using UnityEngine.Networking;

public class Wrapped2D : BaseNetworkBehaviour
{

    [SyncVar]
    public bool InTeleport = false;
    protected Rect? _camRect = null;
    private Rigidbody2D _rigidBody2D;
    private bool _oldIsKinemetic;
    private bool _oldAwake;
    private Vector2 _oldVelocity;

    void Start()
    {
        _rigidBody2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if( isServer)
        {
            if (InTeleport)
            {
                EndTeleport();
            }
            WrapScreen();
        }
    }

    protected void WrapScreen()
    {
        if (!_camRect.HasValue)
        {
            // Cache
            _camRect = GameObjectExt.GetCameraWorldRect(this.gameObject);
        }
        var camRect = _camRect.Value;

        var t = MathfExt.To2D(this.transform.position);

        // If this fails, you did not call base.Start();
        if (camRect.Contains(t))
        {
            return;
        }
        if (t.x > camRect.xMax)
        {
            t.x = camRect.xMin;
        }
        else if (t.x < camRect.xMin)
        {
            t.x = camRect.xMax;
        }
        if (t.y > camRect.yMax)
        {
            t.y = camRect.yMin;
        }
        else if (t.y < camRect.yMin)
        {
            t.y = camRect.yMax;
        }

        StartTeleportTo(MathfExt.From2D(t));
    }

    private void StartTeleportTo(Vector3 toPos)
    {

        //_oldIsKinemetic = _rigidBody2D.isKinematic;
        //oldVelocity = _rigidBody2D.velocity;
        _rigidBody2D.interpolation = RigidbodyInterpolation2D.None;
        //_rigidBody2D.isKinematic = true;
        //_oldAwake = _rigidBody2D.IsAwake();
        //_rigidBody2D.Sleep();
        this.transform.position = toPos;
        InTeleport = true;
    }

    private void EndTeleport()
    {
        //if (_oldAwake)
        //{
        //    _rigidBody2D.WakeUp();
        //}
        //_rigidBody2D.isKinematic = _oldIsKinemetic;
        _rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
       // _rigidBody2D.velocity = _oldVelocity;

    }
}
