using System.Collections;
using System.Collections.Generic;
using Prototype.Inventory;
using Prototype.Script.Test;
using Script.GameSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Gameplay.RoomFacility.Store
{
    public class PlayerWallet : MonoBehaviour
    {
        [SerializeField] private uint _goldNum;
        [SerializeField] private AudioClip _purchaseFail;
        [SerializeField] private AudioClip _purchaseSuccess;
        private PlayerPackage _playerPackage;
        private Text _goldNumText;
        
        // Start is called before the first frame update
        void Start()
        {
            _playerPackage = GetComponent<PlayerPackage>();
            _goldNumText = GameObject.Find("GoldNumber").GetComponent<Text>();
            UpdateGoldCounter();
        }

        public void GainGold(uint gainNum)
        {
            _goldNum += gainNum;
            UpdateGoldCounter();
        }

        public bool Purchase(Item item, uint price)
        {
            if (price > _goldNum)
            {
                SoundManager.Instance.PlayEnvironmentClip(_purchaseFail);
                return false;
            }
            else
            {
                _goldNum -= price;
                UpdateGoldCounter();
                _playerPackage.Inventory.SaveItem(item);
                SoundManager.Instance.PlayEnvironmentClip(_purchaseSuccess);
                return true;
            }
        }

        private void UpdateGoldCounter()
        {
            _goldNumText.text = _goldNum.ToString();
        }
    }
    
}

