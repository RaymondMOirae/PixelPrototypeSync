using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.Inventory
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] private Vector2 InitialVelocity = Vector2.up;
        [SerializeField] private float Acceleration = 1;
        [SerializeField] private float MaxSpeed = 2;
        [SerializeField] private float AngularSpeed = 2;
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
            transform.rotation = Quaternion.identity;
            StopAllCoroutines();
        }

        IEnumerator FlyCoroutine(Transform target)
        {
            var velocity = InitialVelocity;
            
            // Use to avoid dead loop
            var _true = true;
            while(_true)
            {
                var tangent = (target.position - transform.position).ToVector2().normalized;
                var normal = Vector2.Perpendicular(tangent).normalized;
                
                var tangentSpeed = Vector2.Dot(tangent, velocity);
                var normalSpeed = Vector2.Dot(normal, velocity);
                
                normalSpeed =  normalSpeed.SubtractToZero(Acceleration * Time.deltaTime * MathUtility.SignInt(normalSpeed));
                tangentSpeed += Acceleration * Time.deltaTime;
                
                velocity = tangent * tangentSpeed + normal * normalSpeed;
                velocity = Vector2.ClampMagnitude(velocity, MaxSpeed);
                
                transform.Translate(velocity * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.forward, AngularSpeed * Time.deltaTime);
                
                yield return null;
            }
        }

        public void FlyTo(Transform target)
        {
            StartCoroutine(FlyCoroutine(target));
        }

        public void DropAt(Vector3 position)
        {
            transform.position = position;
        }
    }
}