using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Element;
using Prototype.Utils;
using Script.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PixelPalette : SelectGroup<PixelElement>
    {
        public SelectItem pointerItem;
        public SelectItem eraserItem;

        private Dictionary<PixelElement, List<Pixel>> availablePixels = new Dictionary<PixelElement, List<Pixel>>();

        public PixelElement SelectedPixel => (Selected as PixelItem)?.Pixel;
        

        private void Awake()
        {
            
            OnSelectChange += (oldSelection, item) =>
            {
                if (item == pointerItem)
                    PixelEditor.Instance.EditMode = PixelEditMode.None;
                else if (item == eraserItem)
                    PixelEditor.Instance.EditMode = PixelEditMode.Erase;
                else
                    PixelEditor.Instance.EditMode = PixelEditMode.Paint;
            };
            var eraserElement = ScriptableObject.CreateInstance<PixelElement>();
            var pointerElement = ScriptableObject.CreateInstance<PixelElement>();
            AddItem(pointerElement, pointerItem);
            AddItem(eraserElement, eraserItem);
        }

        public void InitPalette(IEnumerable<Pixel> pixels)
        {
            foreach (var pair in this.availablePixels)
            {
                pair.Value.Clear();
                ObjectPool<List<Pixel>>.Release(pair.Value);
                var item = FindSelectItem(pair.Key);
                RemoveItem(pair.Key);
                GameObjectPool.Release(ResourceManager.Instance.PrefabPixelItem, item);
            }
            this.availablePixels.Clear();

            foreach (var pixel in pixels)
            {
                if(!this.availablePixels.ContainsKey(pixel.element))
                    this.availablePixels[pixel.element] = new List<Pixel>(32);
                this.availablePixels[pixel.element].Add(pixel);
            }

            foreach (var pair in this.availablePixels)
            {
                AddPaletteUI(pair.Key, pair.Value.Count);
            }
        }

        void AddPaletteUI(PixelElement pixelElement, int count)
        {
            var item = GameObjectPool.Get<PixelItem>(ResourceManager.Instance.PrefabPixelItem);
            item.UpdateUI(pixelElement, count);
            AddItem(pixelElement, item);
        }

        void UpdatePaletteUI(PixelElement pixelElement)
        {
            FindSelectItem<PixelItem>(pixelElement)?.UpdateUI(pixelElement, this.availablePixels[pixelElement].Count);
        }

        public Pixel TakePixel()
        {
            if (!(SelectedPixel is null) && availablePixels.TryGetValue(SelectedPixel, out var list) && list.NotEmpty())
            {
                var item = Selected as PixelItem;
                item.UpdateUI(item.Pixel, availablePixels[item.Pixel].Count - 1);
                return list.PopBack();
            }

            return null;
        }

        public void SavePixel(Pixel pixel)
        {
            if (pixel?.element is null)
                return;

            if (availablePixels.TryGetValue(pixel.element, out var list))
            {
                list.Add(pixel);
                UpdatePaletteUI(pixel.element);
            }
            else
            {
                var newList = ObjectPool<List<Pixel>>.Get();
                availablePixels.Add(pixel.element, newList);
                
                AddPaletteUI(pixel.element, newList.Count);
            }
        }
    }
}