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

        private PixelElement pixelElement;
        public PixelElement Pixel
        {
            get => pixelElement;
            set
            {
                pixelElement = value;
                if (pixelElement is null)
                {
                    pixelImage.sprite = null;
                    pixelImage.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    pixelImage.sprite = pixelElement.sprite;
                    pixelImage.color = Color.white;
                }
            }
        }

        public void UpdateUI(PixelElement pixel, int count)
        {
            Pixel = pixel;
            this.count = count;
            numberText.text = count.ToString();
            ToggleGroup group;
        }
    }
}