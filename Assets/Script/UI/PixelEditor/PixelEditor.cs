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
        ColorPicker,
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
        private readonly HashSet<Vector2Int> _lockedPixels = new HashSet<Vector2Int>();
        private PixelImage _editingImage = null;

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

        public void SetEditable(Vector2Int pos, bool editable)
        {
            if (!editable)
            {
                _lockedPixels.Add(pos);
                if (slots != null && slots.ContainsIndex(pos) && slots[pos.x, pos.y])
                    slots[pos.x, pos.y].Editable = false;
            }
            else
            {
                _lockedPixels.Remove(pos);
                if (slots != null && slots.ContainsIndex(pos) && slots[pos.x, pos.y])
                    slots[pos.x, pos.y].Editable = true;
            }
        }

        public async Task<PixelImage> Edit(IEnumerable<Pixel> pixels, PixelImage image)
        {
            if (!(_promise is null))
                throw new Exception("Duplicated editor.");

            _editingImage = image;
            UpdateUI(pixels, _editingImage.Size);

            Show();
            
            var promise = new TaskCompletionSource<PixelImage>();
            _promise = promise;

            var result = await promise.Task;
            _editingImage = null;

            Hide();
            
            return result;
        }

        public PixelImage GetImage()
        {
            for(var y = 0 ;  y<size.y;y++)
            for (var x = 0; x < size.x; x++)
            {
                _editingImage.Pixels[x, y] = slots[x, y].Pixel;
            }

            return _editingImage;
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
                    slot.SetPixel(_editingImage.Pixels[x, y]);
                }
            }



            foreach (var lockedSlot in _lockedPixels)
            {
                if (slots.ContainsIndex(lockedSlot))
                    slots[lockedSlot.x, lockedSlot.y].Editable = false;
            }
            
        }
        
        
    }

}