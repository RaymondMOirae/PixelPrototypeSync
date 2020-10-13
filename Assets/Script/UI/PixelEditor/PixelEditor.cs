using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [RequireComponent(typeof(CanvasGroup))]
    public class PixelEditor : GlobalUIPanel<PixelEditor>, ICustomEditorEX
    {
        public PixelPalette palette;
        public GridLayoutGroup pixelCanvas;
        public Button CompleteButton;
        
        public Vector2Int size;
        public int PixelSize = 8;
        public float GridLineWidth;
        public PixelSlot[,] slots;

        public PixelEditMode EditMode = PixelEditMode.None;

        private TaskCompletionSource<PixelImage> _promise = null;

        protected override void Awake()
        {
            base.Awake();
            
            pixelCanvas.gameObject.ClearChildren();
            CompleteButton.onClick.AddListener(() =>
            {
                if(_promise is null)
                    return;
                _promise.SetResult(GetImage());
                _promise = null;
            });
        }

        void UpdateUI(IEnumerable<Pixel> pixels, Vector2Int canvasSize)
        {
            EditMode = PixelEditMode.None;
            palette.InitPalette(pixels);
            size = canvasSize;
            UpdateLayout();
        }

        public async Task<PixelImage> Edit(IEnumerable<Pixel> pixels, Vector2Int canvasSize)
        {
            if (!(_promise is null))
                throw new Exception("Duplicated editor.");
            
            UpdateUI(pixels, canvasSize);

            Show();
            
            var promise = new TaskCompletionSource<PixelImage>();
            _promise = promise;

            var result = await promise.Task;

            Hide();
            
            return result;
        }

        public PixelImage GetImage()
        {
            PixelImage image = new PixelImage(size);
            for(var y = 0 ;  y<size.y;y++)
            for (var x = 0; x < size.x; x++)
            {
                image.Pixels[x, y] = slots[x, y].Pixel;
            }

            return image;
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