using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility
{
    public class InteractiveBase : MonoBehaviour
    {
        private BoxCollider2D _detector;
        private GameObject _interactiveSign;
        public LayerMask _layer;
        public 
        // Start is called before the first frame update
        void Start()
        {
            _detector = transform.Find("Detector").GetComponent<BoxCollider2D>();
            _interactiveSign = transform.Find("Sign").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (_detector.IsTouchingLayers(_layer))
            {
                _interactiveSign.SetActive(true);
            }
            else
            {
                _interactiveSign.SetActive(false);
            }
        }

        public void HandleInteraction()
        {
            Debug.Log("Interact!");

        }
    }

}

