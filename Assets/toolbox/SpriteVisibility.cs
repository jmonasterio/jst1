using Toolbox;
using UnityEngine;

namespace Assets.toolbox
{
    public class SpriteVisibility : BaseNetworkBehaviour
    {
        private bool _visible;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Show(value);
            }
        }

        private void Show( bool show)
        {
            foreach (var sr in GetComponents<SpriteRenderer>())
            {
                sr.enabled = show;
            }
        }
    }
}