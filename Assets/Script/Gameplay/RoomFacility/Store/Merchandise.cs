using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.Inventory;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility.Store
{
    public class Merchandise : MonoBehaviour
    {
        private Merchant _merchant;
        private PlayerWallet _wallet;
        private BoxCollider2D _contactor;
        
        [SerializeField] private uint _price;
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private String _description;
        private Item _merchandiseItemType;

        void Start()
        {
            _merchant = GameObject.Find("Merchant").GetComponent<Merchant>();
            _contactor = GetComponent<BoxCollider2D>();
            _wallet = GameObject.Find("Player").GetComponent<PlayerWallet>();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
            {
                if (_contactor.IsTouchingLayers(_playerLayer))
                {
                    if(_wallet.Purchase(_merchandiseItemType, _price))
                        Destroy(gameObject);
                    else
                        _merchant.ShowDescription("No enough gold.");
                }
                    
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_merchant.IsDisplaying)
                _merchant.ShowDescription(_description);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.CompareTag("Player") && _merchant.IsDisplaying)
                _merchant.HideDescription();
        }
    }
        
}