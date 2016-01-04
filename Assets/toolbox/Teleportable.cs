using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;

namespace Assets.toolbox
{
    /// <summary>
    /// Move an object from place to place without hitting all the physics 2d objet that are in the way.
    /// </summary>
    public class Teleportable : BaseNetworkBehaviour
    {
        private bool InTeleport = false;
        private Rigidbody2D _rigidBody2D;
        private RigidbodyInterpolation2D _oldInterpolate;

        public void Start()
        {
            _rigidBody2D = this.GetComponent<Rigidbody2D>();
        }

        public void Update()
        {
            if (isLocalPlayer || isServer)
            {
                if (InTeleport)
                {
                    EndTeleport();
                }
            }

        }

        public void StartTeleportTo(Vector3 toPos)
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
}
