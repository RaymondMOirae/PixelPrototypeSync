using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.UI;
using Prototype.Script.Test;
using Prototype.Element;
using Prototype.Utils;
using Prototype.Inventory;
using Prototype.Settings;

namespace Prototype.Gameplay.Enemy
{
    public abstract class AttackableBase: MonoBehaviour
    {
        public bool UseHealthBar;
        public bool CanDropPixel;
        protected HealthBar healthBar;
        protected PlayerPackage _package;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected float _spawnRadius;
        [SerializeField] protected List<PixelType> _availableTypes = new List<PixelType>();

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

        protected virtual void InitPackage() 
	    {
            _package = GameObject.Find("Player").GetComponent<PlayerPackage>();
	    }

        public virtual void TakeDamage(string type, float damage)
        {
            currentHealth -= damage;
            healthBar.CurrentHealth = currentHealth;
        }

        protected virtual void DropPixel(PixelType type, Vector2 pos)
	    {
            var droppedItem = GameObjectPool.Get<DroppedItem>(GamePrefabs.Current.DroppedItem);
            droppedItem.Init(new Pixel(type));
            droppedItem.DropAt(pos);
	    }
    }
}

