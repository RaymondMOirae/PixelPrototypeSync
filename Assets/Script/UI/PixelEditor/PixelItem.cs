using Prototype.Element;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.UI
{
    public class PixelItem : SelectItem
    {
        public Image pixelImage;
        public Text numberText;

        private int count;

        private PixelType pixelType;
        public PixelType Pixel
        {
            get => pixelType;
            set
            {
                pixelType = value;
                if (pixelType is null)
                {
                    pixelImage.sprite = null;
                    pixelImage.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    pixelImage.sprite = pixelType.sprite;
                    pixelImage.color = Color.white;
                }
            }
        }

        public void UpdateUI(PixelType pixel, int count)
        {
            Pixel = pixel;
            this.count = count;
            numberText.text = count.ToString();
            ToggleGroup group;
        }
    }
}