using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Utils;
using Prototype.Inventory;


public class InventoryManager : Singleton<InventoryManager>
{
    private Inventory _inventory;
    public static InventoryManager InventoryMgr { get { return Instance; } }

    protected override void Awake()
    {
        base.Awake();
        _inventory = new Inventory();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
