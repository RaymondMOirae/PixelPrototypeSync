using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Utils;
using Script.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    public enum PixelEditMode
    {
        None,
        Paint,
        Erase,
    }
    [RequireComponent(typeof(RectTransform))]
    public class PixelEditor : GlobalUIPanel<PixelEditor>, ICustomEditorEX
    {
        public PixelPalette palette;
        public GridLayoutGroup pixelCanvas;
        public Vector2Int size;
        public float GridLineWidth;
        public PixelSlot[,] slots;

        public PixelEditMode EditMode = PixelEditMode.None;

        protected override void Awake()
        {
            base.Awake();
            
            pixelCanvas.gameObject.ClearChildren();
        }

        public void UpdateUI(IEnumerable<Pixel> pixels, Vector2Int canvasSize)
        {
            EditMode = PixelEditMode.None;
            palette.InitPalette(pixels);
            size = canvasSize;
            UpdateLayout();
        }

        [EditorButton]
        void UpdateLayout()
        {
            if (slots != null)
            {
                foreach (var slot in slots)
                {
                    GameObjectPool.Release(ResourceManager.Instance.PrefabPixelSlot, slot);
                }
            }
            slots = new PixelSlot[size.x, size.y];
            pixelCanvas.cellSize =
                MathUtility.Floor(((pixelCanvas.transform as RectTransform)?.rect.size ?? Vector2.one) / size)
                - Vector2.one * GridLineWidth;
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var slot = GameObjectPool.Get<PixelSlot>(ResourceManager.Instance.PrefabPixelSlot);
                    slot.transform.SetParent(pixelCanvas.transform);
                    slots[x, y] = slot;
                }
            }
        }
        
        
    }

}