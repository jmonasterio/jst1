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
        public const int maxHealth = 3;

        [SyncVar]
        public int health = maxHealth;

        private float _lastDamage;

        public void TakeDamage(int amount)
        {
            if (!isServer)
            {
                return;
            }

            if (Time.time - _lastDamage < 0.3f)
            {
                return;
            }
            _lastDamage = Time.time;

            health -= amount;
            if (health <= 0)
            {
                health = 0;
                Debug.Log("Dead!");
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            var hitCombat = hit.GetComponent<Combat>();
            if (hitCombat != null)
            {
                if (hit.transform.position.y - this.transform.position.y > 0.01f)
                {
                    this.TakeDamage(1);
                }
                else if (this.transform.position.y - hit.transform.position.y > 0.01f)
                {
                    hitCombat.TakeDamage(1);
                }
                else
                {
                    // tie
                }
            }
        }
    }
}
