using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackstraw : EnemyBase
{
    private void Awake()
    {
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    public override void TakeDamage(string type, int damage)
    {
        base.TakeDamage(type, damage);
        if(currentHealth == 0)
        {
            currentHealth = maxHealth;
            healthBar.RecoverMaxHealth();
        }
        Debug.Log("Damage:" + type + " received by Jackstraw");
    }
}
