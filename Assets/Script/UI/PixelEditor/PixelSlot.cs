

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

        public Vector2Int Position;

        private void Awake()
        {
            SetPixel(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Editable)
            {
                PopupMessage.Show("The pixel is protected and cannot be modified.");
                return;
            }
            
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

        internal void Setup(Vector2Int position)
        {
            Position = position;
        }

        internal void SetPixel(Pixel pixel)
        {
            if (pixel && pixel.Protected)
                Editable = false;
            
            if (pixel?.Type is null)
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
            var pixel = PixelEditor.Instance.Inventory.Take<Pixel>(PixelEditor.Instance.PixelToPaint);
            if (!pixel)
                return;

            var action = new UndoAction()
            {
                Position = this.Position,
                EditMode = PixelEditMode.Paint,
                NewPixel = pixel,
                OldPixel = Pixel,
            };
            
            PixelEditor.Instance.RecordAction(action);

            PixelEditor.Instance.Inventory.SaveItem(Pixel);
            SetPixel(pixel);
        }

        void Erase()
        {
            if (Pixel?.Type is null)
                return;
            
            
            var action = new UndoAction()
            {
                Position = this.Position,
                EditMode = PixelEditMode.Paint,
                NewPixel = null,
                OldPixel = Pixel,
            };
            PixelEditor.Instance.RecordAction(action);
            
            PixelEditor.Instance.Inventory.SaveItem(Pixel);
            SetPixel(null);
        }
        
    }
}