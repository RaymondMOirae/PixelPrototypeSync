using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class DynamicOverlay: MonoBehaviour
{
    public float near = 0;
    public float far = 10;
    public float lerpFriction;
    public float lowerRegion;
    
    private void UpdateIntervalSpace(Vector2 region)
    {
        lerpFriction = (far - near) / region.y;
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
