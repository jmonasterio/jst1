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

        public AudioClip LanceSound;
        public AudioClip BumpSound;

        [SyncVar] public int health = maxHealth;

        private float _lastDamage;

        public void Tie()
        {
            SafeGameManager.PlayClip(LanceSound);
        }

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
            SafeGameManager.PlayClip(BumpSound);
            health -= amount;
            if (health <= 0)
            {
#if VERBOSE
                Debug.Log("Dead!");
#endif
                health = maxHealth;

                // Called on the server, will be invoked on the clients.
                RpcRespawn();
            }
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                var player = this.GetComponent<Player>();

                SafeGameManager.SceneController.KillPlayer(player);
            }
            else
            {
                var enemy = this.GetComponent<Enemy>();
                SafeGameManager.SceneController.RespawnEnemy( enemy);
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
                    this.Tie();
                }
            }
        }
    }
}
