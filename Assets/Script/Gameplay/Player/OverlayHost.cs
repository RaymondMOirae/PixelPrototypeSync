using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverlayHost : MonoBehaviour
{
    public Vector2 verticalRegion; // (lower y coordinate, full vertical space)
    
    // Start is called before the first frame update
    void Start()
    {
        Transform t = transform.Find("Wall");
        Bounds bounds = t.GetComponent<TilemapRenderer>().bounds;

        verticalRegion.x = bounds.min.y;
        verticalRegion.y = bounds.size.y;

        Transform units = transform.Find("CombatUnits");
        units.BroadcastMessage("UpdateIntervalSpace", verticalRegion);
    }

}
