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

        public void Grab(GameObject picker)
        {
            if (!isServer)
            {
                return;
            }

            SafeGameManager.PlayClip(PickupSound);
            PickedUp = this.gameObject;
            Destroy( PickedUp, 0.01f);
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