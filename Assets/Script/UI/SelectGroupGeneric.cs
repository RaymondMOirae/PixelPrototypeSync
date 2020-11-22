using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class SelectGroup<T> : MonoBehaviour where T : class
    {
        public RectTransform container;
        private readonly Dictionary<T, SelectItem> _itemsByKey = new Dictionary<T, SelectItem>();
        private readonly Dictionary<SelectItem, T> _keysByItem = new Dictionary<SelectItem, T>();
        private readonly List<T> _keyLists = new List<T>();
        public SelectItem SelectedItem { get; private set; }
        public T SelectedKey { get; private set; }

        public IEnumerable<SelectItem> Items => _itemsByKey.Values;

        public delegate void SelectionChangeEventHandler(T oldKey, T newKey);

        public event SelectionChangeEventHandler OnSelectChange; 

        private void Reset()
        {
            container = transform as RectTransform;
        }

        public void AddItem(T key, SelectItem item)
        {
            item.transform.SetParent(container);
            _itemsByKey.Add(key, item);
            _keysByItem.Add(item, key);
            item.OnSelected += ItemOnSelected;
            _keyLists.Add(key);
        }

        private void ItemOnSelected(SelectItem item)
        {
            SelectItem(item);
        }

        public void SelectItem(SelectItem item)
        {
            var old = SelectedItem;
            if(SelectedItem)
                SelectedItem.DeselectInternal();
            
            SelectedItem = item;
            SelectedKey = _keysByItem[item];

            OnSelectChange?.Invoke(
                old ? _keysByItem[old] : null,
                item ? _keysByItem[item] : null
            );

        }

        public void SelectKey(T key)
        {
            SelectItem(_itemsByKey[key]);
        }

        public void SelectIndex(int index)
        {
            if (0 <= index && index < _keyLists.Count)
            {
                SelectKey(_keyLists[index]);
            }
            else
                SelectItem(null);
        }

        public SelectItem FindSelectItem(T key)
        {
            return _itemsByKey[key];
        }

        public TValue FindSelectItem<TValue>(T key) where TValue : SelectItem
            => _itemsByKey[key] as TValue;

        public void RemoveItem(T key)
        {
            if (_itemsByKey.TryGetValue(key, out var item))
            {
                item.OnSelected -= ItemOnSelected;
                _itemsByKey.Remove(key);
                _keysByItem.Remove(item);
                _keyLists.Remove(key);
                if (SelectedItem == item)
                {
                    SelectItem(null);
                }
            }
        }

        public void RemoveItem(SelectItem item)
        {
            foreach (var pair in _itemsByKey)
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