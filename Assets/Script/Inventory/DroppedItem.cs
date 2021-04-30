using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.Inventory
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] private Vector2 InitialVelocity = Vector2.up;
        [SerializeField] private float Acceleration = 1;
        [SerializeField] private float MaxSpeed = 2;
        [SerializeField] private float AngularSpeed = 2;
        [SerializeField] private float SpreadRadius = 1;
        [SerializeField] private float DropAnimationDuration = 1;
        [SerializeField] private AnimationCurve DropPositionCurve;
        public Item Item { get; private set; }
        public bool Pickable { get; private set; }
        

        private SpriteRenderer _spriteRenderer;

        private void Reset()
        {
            DropPositionCurve.keys = new[]
            {
                new Keyframe(0, 0),
                new Keyframe(1, 0)
            };
        }

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

        IEnumerator DropCoroutine(Vector3 position)
        {
            transform.position = position;
            var targetPos = Random.insideUnitCircle * SpreadRadius;
            foreach (var t in Utility.TimerNormalized(DropAnimationDuration))
            {
                transform.position = new Vector3(
                    position.x + Mathf.Lerp(0, targetPos.x, t),
                    position.y + Mathf.Lerp(0, targetPos.y, t) + DropPositionCurve.Evaluate(t),
                    position.z
                );
                yield return null;
            }

            Pickable = true;
        }

        public void FlyTo(Transform target)
        {
            StartCoroutine(FlyCoroutine(target));
        }

        public void DropAt(Vector3 position)
        {
            Pickable = false;
            StartCoroutine(DropCoroutine(position));
        }
    }
}