

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

        private Pixel pixel;

        private void Awake()
        {
            SetPixel(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (PixelEditor.Instance.EditMode)
            {
                case PixelEditMode.Paint:
                    PixelEditor.Instance.palette.SavePixel(pixel);
                    SetPixel(PixelEditor.Instance.palette.TakePixel());
                    break;
                case PixelEditMode.Erase:
                    PixelEditor.Instance.palette.SavePixel(pixel);
                    SetPixel(null);
                    break;
            }
        }

        public void SetPixel(Pixel pixel)
        {
            if (pixel is null)
            {
                pixelImage.sprite = null;
                pixelImage.color = Color.white.Transparent();
            }
            else
            {
                pixelImage.sprite = pixel.element.sprite;
                pixelImage.color = Color.white;
            }

            this.pixel = pixel;
        }
    }
}