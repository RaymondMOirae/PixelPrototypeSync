using System;
using Prototype.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype.Gameplay.Player
{
    public class ItemPicker : MonoBehaviour
    {
        public class PickEvent : UnityEvent<DroppedItem>
        {
        }

        [SerializeField] public PickEvent OnItemPicked = new PickEvent();
        [SerializeField] private float PickRadius;
        [SerializeField]
        [HideInInspector]
        public int LayerMask;
        
        readonly Collider2D[] _collector = new Collider2D[64];
        private void Update()
        {
            var count = Physics2D.OverlapCircleNonAlloc(transform.position, PickRadius, _collector, 1 << LayerMask);
            for (var i = 0; i < count; i++)
            {
                if (_collector[i].GetComponent<DroppedItem>() is var item)
                {
                    OnItemPicked.Invoke(item);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, PickRadius);
        }
    }
}