using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyBase: MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth;
    public int currentHealth;

    public virtual void InitComponents()
    {
        healthBar = transform.Find("UnitCanvas").GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        healthBar.RecoverMaxHealth();
    }

    public virtual void TakeDamage(string type, int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

}
