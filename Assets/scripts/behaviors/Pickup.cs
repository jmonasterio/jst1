using Toolbox;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts.behaviors
{
    class Pickup : BaseNetworkBehaviour
    {
        public AudioClip PickupSound;

        [SyncVar]
        public GameObject PickedUp = null;

        public void Grab(GameObject pickup)
        {
            if (!isServer)
            {
                return;
            }

            SafeGameManager.PlayClip(PickupSound);
            PickedUp = pickup;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            this.Grab(hit);
        }
    }
}