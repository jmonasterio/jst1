using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox;
using UnityEngine;

namespace Assets.scripts.behaviors
{
    public class HealthBar : BaseBehaviour
    {
        public Sprite Health0; // Not really used.
        public Sprite Health1;
        public Sprite Health2;
        public Sprite Health3;

        private Combat _combat;
        private SpriteRenderer _spriteRender;

        void Awake()
        {
            _combat = GetComponentInParent<Combat>();
            _spriteRender = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (_combat != null)
            {
                if (_combat.health == 3)
                {
                    _spriteRender.sprite = Health3;
                }
                else if (_combat.health == 2)
                {
                    _spriteRender.sprite = Health2;
                }
                else if (_combat.health == 1)
                {
                    _spriteRender.sprite = Health1;
                }
                else
                {
                    _spriteRender.sprite = Health0;
                }
            }
        }

    }
}
