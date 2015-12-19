using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts.behaviors
{
    class Combat : BaseNetworkBehaviour
    {
        public const int maxHealth = 100;

        [SyncVar] public int health = maxHealth;

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            health -= amount;
            if (health <= 0)
            {
                health = 0;
                Debug.Log("Dead!");
            }
        }

        void OnCollisionEnter(Collision2D collision)
        {
            var hit = collision.gameObject;
            var hitCombat = hit.GetComponent<Combat>();
            if (hitCombat != null)
            {
                if (hit.transform.position.y > this.transform.position.y)
                {
                    hitCombat.TakeDamage(10);
                    Destroy(gameObject);
                }
            }
        }
    }
}
