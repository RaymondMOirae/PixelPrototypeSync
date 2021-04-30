using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility
{
    public class Door : MonoBehaviour
    {
        public LayerMask playerLayer;
        private BoxCollider2D _box;
        private Animator _animator;
        private OverlayHost _overlayHost;
        // Start is called before the first frame update
        void Start()
        {
            _box = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            _overlayHost = transform.parent.GetComponent<OverlayHost>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_box.IsTouchingLayers(playerLayer))
            {
                _animator.SetBool("PlayerAround", true);
            }
            else
            {
                _animator.SetBool("PlayerAround", false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.BroadcastMessage("UpdateIntervalSpace", _overlayHost.verticalRegion);
            }
        }


    }
}

