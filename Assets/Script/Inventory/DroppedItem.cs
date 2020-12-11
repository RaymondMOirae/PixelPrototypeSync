using System;
using UnityEngine;

namespace Prototype.Inventory
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class DroppedItem : MonoBehaviour
    {
        public Item Item { get; private set; }

        private SpriteRenderer _spriteRenderer; 
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Init(Item item)
        {
            Item = item;
            _spriteRenderer.sprite = item.ItemType.DroppedSprite;
        }

        public void DropAt(Vector3 position)
        {
            transform.position = position;
        }
    }
}