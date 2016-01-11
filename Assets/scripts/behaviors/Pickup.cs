using System;
using Toolbox;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts.behaviors
{
    class Pickup : BaseNetworkBehaviour
    {
        public AudioClip PickupSound;
        public event TypedEventHandler.EventHandler<Pickup> PickedUp;

        [SyncVar]
        public GameObject PickedUpItem = null;

        [SyncVar]
        public GameObject PickedUpBy = null;

        public void Grab(GameObject picker)
        {
            if (!isServer)
            {
                return;
            }

            SafeGameManager.PlayClip(PickupSound);
            if (PickedUp != null)
            {
                PickedUpItem = this.gameObject;
                PickedUpBy = picker;
                PickedUp(this, new EventArgs());
            }
            else
            {
                // Event not handled, so let's just destroy ourselves.
                Destroy(PickedUpItem, 0.01f);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var bird = collision.gameObject.GetComponent<Bird>();
            if (bird != null)
            {
                var hit = collision.gameObject;
                this.Grab(hit);
            }
        }
    }
}