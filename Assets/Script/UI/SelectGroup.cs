using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class SelectGroup<T> : MonoBehaviour where T : class
    {
        public RectTransform container;
        private Dictionary<T, SelectItem> items = new Dictionary<T, SelectItem>();
        public SelectItem Selected { get; private set; }

        public IEnumerable<SelectItem> Items => items.Values;

        public event Action<SelectItem, SelectItem> OnSelectChange; 

        private void Reset()
        {
            container = transform as RectTransform;
        }

        public void AddItem(T key, SelectItem item)
        {
            item.transform.SetParent(container);
            items.Add(key, item);
            item.OnSelected += ItemOnSelected;
        }

        private void ItemOnSelected(SelectItem item)
        {
            SelectItem(item);
        }

        public void SelectItem(SelectItem item)
        {
            var old = Selected;
            if(Selected)
                Selected.DeselectInternal();
            
            Selected = item;
            
            OnSelectChange?.Invoke(old, item);

        }

        public void SelectKey(T key)
        {
            SelectItem(items[key]);
        }

        public SelectItem FindSelectItem(T key)
        {
            return items[key];
        }

        public TValue FindSelectItem<TValue>(T key) where TValue : SelectItem
            => items[key] as TValue;

        public void RemoveItem(T key)
        {
            if (items.TryGetValue(key, out var item))
            {
                item.OnSelected -= ItemOnSelected;
                items.Remove(key);
                if (Selected == item)
                {
                    SelectItem(null);
                }
            }
        }

        public void RemoveItem(SelectItem item)
        {
            foreach (var pair in items)
            {
                if (pair.Value == item)
                {
                    RemoveItem(pair.Key);
                    return;
                }
                
            }
            
        }
    }
}