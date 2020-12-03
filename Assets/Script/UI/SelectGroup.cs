using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class SelectGroup : MonoBehaviour
    {
        [SerializeField]
        private bool LoadItemsFromChildren = false;
        public RectTransform container;
        private readonly List<SelectItem> _selectItems = new List<SelectItem>();
        public SelectItem SelectedItem { get; private set; }
        public int SelectedIndex { get; private set; }

        public bool HasSelected => SelectedIndex >= 0;

        public IEnumerable<SelectItem> Items => _selectItems;

        public delegate void SelectionChangeEventHandler(int oldIndex, int newIndex);

        public event SelectionChangeEventHandler OnSelectChange; 

        private void Reset()
        {
            container = transform as RectTransform;
        }

        private void Awake()
        {
            if (LoadItemsFromChildren)
            {
                foreach (var child in GetComponentsInChildren<SelectItem>())
                    AddItem(child);
            }
        }

        public int AddItem(SelectItem item)
        {
            item.transform.SetParent(container, false);
            _selectItems.Add(item);
            item.OnSelected += ItemOnSelected;
            return _selectItems.Count - 1;
        }

        private void ItemOnSelected(SelectItem item)
        {
            SelectItem(item);
        }

        public void SelectItem(SelectItem item)
        {
            SelectByIndex(_selectItems.IndexOf(item));
        }

        public SelectItem ItemByIndex(int index)
        {
            if (index < 0 || index >= _selectItems.Count)
                return null;
            return _selectItems[index];
        }

        public void SelectByIndex(int index)
        {
            var old = SelectedIndex;
            if(SelectedItem)
                SelectedItem.SetDeselectStyle();

            if (index >= 0)
                SelectedItem = _selectItems[index];
            else
                SelectedItem = null;
            
            if(SelectedItem)
                SelectedItem.SetSelectStyle();
            

            OnSelectChange?.Invoke(old, SelectedIndex);
        }

        public void RemoveItem(int index)
        {
            if (HasSelected)
                SelectByIndex(-1);
            if (index >= 0 && index < _selectItems.Count)
                _selectItems.RemoveAt(index);
        }

        public void RemoveItem(SelectItem item)
        {
            RemoveItem(_selectItems.IndexOf(item));
        }
    }
}