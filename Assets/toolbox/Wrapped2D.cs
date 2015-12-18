using UnityEngine;
using System.Collections;
using Toolbox;

public class Wrapped2D : BaseNetworkBehaviour
{

    protected Rect? _camRect = null;

    // Update is called once per frame
    void Update()
    {
        WrapScreen();
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

        // Teleport better?
        this.transform.position = MathfExt.From2D(t);
    }
}
