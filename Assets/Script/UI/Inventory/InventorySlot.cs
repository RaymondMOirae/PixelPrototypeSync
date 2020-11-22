using System.Collections;
using Prototype.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.UI.Inventory
{
    [RequireComponent(typeof(RectTransform))]
    public class InventorySlot : SelectItem, IPointerEnterHandler, IPointerExitHandler
    {
        public bool EnableGrouping = true;

        [SerializeField] private float DetailsDelay = 1f;

        [SerializeField] private Text GroupText;

        [SerializeField] private Image ItemImage;
        
        
        private ItemGroup _itemGroup;
        public ItemGroup ItemGroup
        {
            get => _itemGroup;
            set
            {
                _itemGroup = value;
                UpdateUI(value);
            }
        }

        public void UpdateUI(ItemGroup itemGroup)
        {
            _itemGroup = itemGroup;
            if (!itemGroup)
            {
                ItemImage.sprite = null;
                ItemImage.color = Color.white.Transparent();
                GroupText.text = "0";
                return;
            }
            

            ItemImage.sprite = itemGroup.ItemType.Image;
            ItemImage.color = Color.white;
            GroupText.text = itemGroup.Count.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("enter");
            StartCoroutine(WaitAndShowDetails());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("exit");
            StopAllCoroutines();
            DetailPanelManager.HideAllPanel();
        }

        IEnumerator WaitAndShowDetails()
        {
            yield return new WaitForSeconds(DetailsDelay);
            DetailPanelManager.ShowPanel(_itemGroup.ItemType);
        }
    }
}