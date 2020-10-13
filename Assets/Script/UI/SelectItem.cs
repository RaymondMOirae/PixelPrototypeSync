using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prototype.UI
{
    public class SelectItem : MonoBehaviour, IPointerClickHandler
    {
        public Image styleTarget;
        public Color defaultColor = Color.white;
        public Color selectedColor = Color.white;
        
        internal event Action<SelectItem> OnSelected;

        private bool selected;

        private void OnEnable()
        {
            UpdateStyle();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            SelectInternal();
        }

        internal void SelectInternal()
        {
            selected = true;
            UpdateStyle();
            OnSelected?.Invoke(this);
            OnSelect();
        }

        internal void DeselectInternal()
        {
            selected = false;
            UpdateStyle();
            OnDeselect();
        }

        protected virtual void OnSelect()
        {
        }

        protected virtual void OnDeselect()
        {
        }

        private void UpdateStyle()
        {
            if (selected && styleTarget)
                styleTarget.color = selectedColor;
            else if (styleTarget)
                styleTarget.color = defaultColor;
        }
    }
}