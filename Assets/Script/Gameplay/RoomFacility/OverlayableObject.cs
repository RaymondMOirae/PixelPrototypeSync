using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Prototype.Gameplay
{
    [RequireComponent(typeof(Transform))]
    public class OverlayableObject: MonoBehaviour
    {
        public static float NearZDepth { get; set; } = 0;

        public static float FarZDepth { get; set; } = 10;

        public float lerpFriction;
        public float lowerRegion;
        
        private void UpdateIntervalSpace(Vector2 region)
        {
            lerpFriction = (FarZDepth - NearZDepth) / region.y;
            lowerRegion = region.x;

        }

        // Update is called once per frame
        void Update()
        {
            Vector3 pos = transform.position;
            pos.z = (transform.position.y - lowerRegion) * lerpFriction;
            transform.position = pos;
        }
    }

}

