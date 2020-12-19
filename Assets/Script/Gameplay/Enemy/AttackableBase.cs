using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.UI;

namespace Prototype.Gameplay.Enemy
{
    public abstract class AttackableBase: MonoBehaviour
    {
        protected HealthBar healthBar;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;

        protected virtual void InitHealthBar()
        {
            FindHealthBar();
            currentHealth = maxHealth;
            healthBar.MaxHealth = maxHealth;
            healthBar.RestoreMaxHealth();
        }

        protected virtual void FindHealthBar()
        {
            healthBar = transform.Find("UnitCanvas").GetComponentInChildren<HealthBar>();
        }

        public virtual void TakeDamage(string type, float damage)
        {
            currentHealth -= damage;
            healthBar.CurrentHealth = currentHealth;
        }

    }

}

