using System.Collections;
using System.Collections.Generic;
using Prototype.Inventory;
using Prototype.Script.Test;
using UnityEditor;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility.Store
{
    public class PlayerWallet : MonoBehaviour
    {
        [SerializeField] private uint _goldNum;
        private PlayerPackage _playerPackage;
        
        // Start is called before the first frame update
        void Start()
        {
            _playerPackage = GetComponent<PlayerPackage>();
        }

        public bool Purchase(Item item, uint price)
        {
            if (price > _goldNum)
            {
                return false;
            }
            else
            {
                _goldNum -= price;
                _playerPackage.Inventory.SaveItem(item);
                return true;
            }
        }
    }
    
}

