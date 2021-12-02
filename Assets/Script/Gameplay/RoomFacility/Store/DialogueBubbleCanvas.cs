using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Gameplay.RoomFacility.Store
{
    public class DialogueBubbleCanvas : MonoBehaviour
    {
        private Text _contentText;
        // Start is called before the first frame update
        void Awake()
        {
            _contentText = GetComponentInChildren<Text>();
        }

        public void DisplayText(String content)
        {
            _contentText.text = content;
            gameObject.SetActive(true);
        }

        public void EndDisplay()
        {
            gameObject.SetActive(false);
        }

    }
        
}
