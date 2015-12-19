using UnityEngine;
using System.Collections;
using Toolbox;
using UnityEngine.Networking;

/// <summary>
/// Wraps object around to other side of screen (left/right and top/bottom). Will turn off physics while
///     teleporting the player, so the telportation doesn't collide. Seems to work on network.
/// </summary>
public class Wrapped2D : BaseNetworkBehaviour
{
    public bool VerticalWrap = false;

    private bool InTeleport = false;
    private Rect? _camRect = null;
    private Rigidbody2D _rigidBody2D;
    private RigidbodyInterpolation2D _oldInterpolate;

    void Start()
    {
        _rigidBody2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if( isLocalPlayer)
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
        if (VerticalWrap)
        {
            if (t.y > camRect.yMax)
            {
                t.y = camRect.yMin;
            }
            else if (t.y < camRect.yMin)
            {
                t.y = camRect.yMax;
            }
        }

        StartTeleportTo(MathfExt.From2D(t));
    }

    private void StartTeleportTo(Vector3 toPos)
    {
        // Normally you want interpolation on, so you have smooth movement while networking.
        // But, while teleporting to other side of screen, you really don't want to interpolate because
        //  you'll collide with objects between current position and other side.
        _oldInterpolate = _rigidBody2D.interpolation;
        _rigidBody2D.interpolation = RigidbodyInterpolation2D.None;

        // No position.
        this.transform.position = toPos;

        // Notify server (if necessary) that this player is teleporting. Server will end the telport.
        InTeleport = true;
    }

    private void EndTeleport()
    {
        _rigidBody2D.interpolation = _oldInterpolate;
    }
}
