using UnityEngine;
using System.Collections;
using Assets.toolbox;
using Toolbox;
using UnityEngine.Networking;

/// <summary>
/// Wraps object around to other side of screen (left/right and top/bottom). Will turn off physics while
///     teleporting the player, so the telportation doesn't collide. Seems to work on network.
/// </summary>
public class Wrapped2D : BaseNetworkBehaviour
{
    public bool VerticalWrap = false;

    private Rect? _camRect = null;
    private Teleportable _teleportable;

    void Start()
    {
        _teleportable = this.GetComponent<Teleportable>();
    }

    // Update is called once per frame
    void Update()
    {
        if( isLocalPlayer || isServer)
        {
            WrapScreen();
        }
    }

    // Assumes the camera rectangle doesn't change on each frame.
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

        _teleportable.StartTeleportTo(MathfExt.From2D(t));
    }

}
