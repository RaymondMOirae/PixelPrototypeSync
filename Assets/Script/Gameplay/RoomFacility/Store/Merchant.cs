using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility.Store
{
    public class Merchant : MonoBehaviour
    {
        
        private bool _isDisplaying;
        private DialogueBubbleCanvas _descriptionCanvas;
        public bool IsDisplaying => _isDisplaying;
        // Start is called before the first frame update
        void Start()
        {
            _descriptionCanvas = GetComponentInChildren<DialogueBubbleCanvas>();
            _descriptionCanvas.EndDisplay();
            _isDisplaying = false;
        }

        public void ShowDescription(String desc)
        {
            _isDisplaying = true;
            _descriptionCanvas.DisplayText(desc);
        }

        public void HideDescription()
        {
            _descriptionCanvas.EndDisplay();
            _isDisplaying = false;
        }
    }
    
}