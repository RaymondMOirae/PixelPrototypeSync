using System;
using System.Collections.Generic;
using Prototype.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype.Gameplay.Player
{
    public class ItemPicker : MonoBehaviour
    {
        [Serializable]
        public class PickEvent : UnityEvent<DroppedItem>
        {
        }

        [SerializeField] public PickEvent OnItemPicked = new PickEvent();
        [SerializeField] private float PickRadius = .6f;
        [SerializeField] private float MagnetRadius = 2;
        [SerializeField] [HideInInspector] public int LayerMask;
        
        private readonly HashSet<DroppedItem> _attractingItem = new HashSet<DroppedItem>();
        
        
        
        readonly Collider2D[] _collector = new Collider2D[64];
        private void Update()
        {
            var count = Physics2D.OverlapCircleNonAlloc(transform.position, MagnetRadius, _collector, 1 << LayerMask);
            for (var i = 0; i < count; i++)
            {
                if (_collector[i].GetComponent<DroppedItem>() is var item && !_attractingItem.Contains(item) && item.Pickable)
                {
                    _attractingItem.Add(item);
                    item.FlyTo(transform);
                }
            }
            
            count = Physics2D.OverlapCircleNonAlloc(transform.position, PickRadius, _collector, 1 << LayerMask);
            for (var i = 0; i < count; i++)
            {
                if (_collector[i].GetComponent<DroppedItem>() is var item && item.Pickable)
                {
                    _attractingItem.Remove(item);
                    OnItemPicked.Invoke(item);
                }
            }
        }
        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, PickRadius);
            Gizmos.DrawWireSphere(transform.position, MagnetRadius);
        }
    }
}