

using System;
using Prototype.Element;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.UI
{
    public class PixelSlot : MonoBehaviour, IPointerClickHandler
    {
        public Image pixelImage;

        public Image highlight;

        public Pixel Pixel { get; private set; }

        public bool Editable = true;

        private void Awake()
        {
            SetPixel(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!Editable)
                return;
            
            switch (PixelEditor.Instance.EditMode)
            {
                case PixelEditMode.Paint:
                    Paint();
                    break;
                case PixelEditMode.Erase:
                    Erase();
                    break;
            }
        }

        public void SetPixel(Pixel pixel)
        {
            if (pixel && pixel.Protected)
                Editable = false;
            
            if (pixel is null)
            {
                pixelImage.sprite = null;
                pixelImage.color = Color.white.Transparent();
            }
            else
            {
                pixelImage.sprite = pixel.Type.sprite;
                pixelImage.color = Color.white;
            }

            this.Pixel = pixel;
        }

        void Paint()
        {
            var pixel = PixelEditor.Instance.Inventory.Pixels.Take<Pixel>(PixelEditor.Instance.PixelToPaint);
            if (!pixel)
                return;
            
            PixelEditor.Instance.Inventory.Pixels.SaveItem(Pixel);
            SetPixel(pixel);
        }

        void Erase()
        {
            PixelEditor.Instance.Inventory.Pixels.SaveItem(Pixel);
            SetPixel(null);
        }
        
    }
}