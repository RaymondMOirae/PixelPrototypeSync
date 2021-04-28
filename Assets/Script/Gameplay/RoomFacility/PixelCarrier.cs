using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Prototype.Inventory;
using Prototype.Script.Test;
using Prototype.Utils;
using Prototype.Element;
using Prototype.Gameplay.Enemy;


public class PixelCarrier :AttackableBase
{


    //public Dictionary<PixelType,>

    private void Start()
    {
        _package = GameObject.Find("Player").GetComponent<PlayerPackage>();
    }

    public override void TakeDamage(string type, float damage) 
    { 
		Vector2 offset = Random.insideUnitCircle * _spawnRadius;
		var pos =  transform.position + MathUtility.ToVector3(offset);
		_package.Inventory.DropOne(_availableTypes[(int)(Random.value * _availableTypes.Count)], pos);
    }
}
