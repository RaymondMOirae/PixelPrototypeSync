using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.UI;

namespace Prototype.Gameplay.Enemy
{
    public abstract class EnemyBase: MonoBehaviour
    {
        public HealthBar healthBar;
        public float maxHealth;
        public float currentHealth;

        public virtual void InitComponents()
        {
            healthBar = transform.Find("UnitCanvas").GetComponentInChildren<HealthBar>();
            healthBar.SetMaxHealth(maxHealth);
            healthBar.RecoverMaxHealth();
        }

        public virtual void TakeDamage(string type, float damage)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }

    }

}

