using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class Jackstraw : AttackableBase
    {
        private void Awake()
        {
            InitHealthBar();
            healthBar.MaxHealth = maxHealth;
            currentHealth = maxHealth;
        }

        public override void TakeDamage(string type, float damage)
        {
            base.TakeDamage(type, damage);
            if(currentHealth <= 0.0f)
            {
                currentHealth = maxHealth;
                healthBar.RestoreMaxHealth();
            }
            Debug.Log("Damage:" + type  + " " + damage.ToString() + " received by Jackstraw");
        }
    }
}

