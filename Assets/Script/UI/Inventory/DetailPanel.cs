using System.Threading.Tasks;
using Prototype.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI.Inventory
{
    [RequireComponent(typeof(RectTransform))]
    public class DetailPanel : UIPanel
    {
        [SerializeField] private Image ItemImage;
        [SerializeField] private Image PreviewBackground;
        [SerializeField] private Text NameText;
        [SerializeField] private Text PropertiesText;
        [SerializeField] private Text DescriptionText;
        
        public RectTransform RectTransform => (transform as RectTransform);

        public Vector2 Size => RectTransform.rect.size;
        
        public void Show(ItemType itemType, Vector2 position)
        {
            ItemImage.sprite = itemType.InventoryIcon;
            ItemImage.color = Color.white;
            NameText.text = itemType.ItemName;
            PropertiesText.text = itemType.Properties;
            DescriptionText.text = itemType.Description;
            if (itemType.InventoryBackground)
            {
                PreviewBackground.sprite = itemType.InventoryBackground;
                PreviewBackground.color = Color.white;
            }
            else
            {
                PreviewBackground.sprite = null;
                PreviewBackground.color = Color.white.Transparent();
            }
            
            var rectTrans = transform as RectTransform;
            rectTrans.anchoredPosition = position;
            Show();
        }

        public void ResetPivot()
        {
            (transform as RectTransform).pivot = new Vector2(0, 1);
        }

        public void MovePivotBottom()
        {
            var pivot = RectTransform.pivot;
            pivot.y = 0;
            RectTransform.pivot = pivot;
        }

        public void MovePivotRight()
        {
            var pivot = RectTransform.pivot;
            pivot.x = 1;
            RectTransform.pivot = pivot;
        }
    }
}