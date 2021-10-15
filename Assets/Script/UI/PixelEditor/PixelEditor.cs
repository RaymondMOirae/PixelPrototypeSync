using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.Element;
using Prototype.Inventory;
using Prototype.Settings;
using Prototype.UI.Inventory;
using Prototype.Utils;
using Script.GameSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Prototype.UI
{
    public enum PixelEditMode: int
    {
        None = 0,
        Paint,
        Erase,
        ColorPicker,
    }
    [RequireComponent(typeof(CanvasGroup))]
    public class PixelEditor : GlobalUIPanel<PixelEditor>, ICustomEditorEX
    {
    
        [SerializeField]
        private Resources _resources;
        
        public InventoryPanel PalettePanel;

        public InventoryPanel TemplatesPanel;

        [SerializeField] private SelectItem EditModeNone;
        [SerializeField] private SelectItem EditModePaint;
        [SerializeField] private SelectItem EditModeErase;
        [SerializeField] private SelectItem EditModeColorPicker;

        [SerializeField] private SelectGroup EditModeSelectGroup;

        [SerializeField] private Text WeaponInfoOverall;
        [SerializeField] private Text WeaponInfoLeft;
        [SerializeField] private Text WeaponInfoRight;
        [SerializeField] private Text WeaponInfoSpike;

        public Prototype.Inventory.Inventory Inventory { get; private set; }

        // public PixelPalette palette;
        public GridLayoutGroup pixelCanvas;
        public Button CompleteButton;
        
        public Vector2Int CanvasSize => _editingImage.Size;
        public float GridLineWidth;
        public PixelSlot[,] slots;

        public PixelEditMode EditMode = PixelEditMode.None;

        private TaskCompletionSource<int> _promise = null;
        // private readonly HashSet<Vector2Int> _lockedPixels = new HashSet<Vector2Int>();
        private PixelWeapon _editingImage = null;
        private PixelWeapon _analyseWeapon = null;
        private PixelWeaponAnalyser _weaponAnalyser = null;
        private readonly Stack<UndoAction> _undoStack = new Stack<UndoAction>();
        private readonly Stack<UndoAction> _redoStack = new Stack<UndoAction>();

        public PixelType PixelToPaint => PalettePanel.SelectedKey.ItemType as PixelType;

        protected override void Awake()
        {
            base.Awake();
            
            pixelCanvas.gameObject.ClearChildren();
            CompleteButton.onClick.AddListener(() =>
            {
                if(_promise is null)
                    return;
                Done();
            });
            
            TemplatesPanel.OnSelectChange += TemplatesPanelOnOnSelectChange;
            PalettePanel.OnSelectChange += (old, newKey) =>
            {
                if (newKey)
                {
                    EditMode = PixelEditMode.Paint;
                    EditModeSelectGroup.SelectItem(EditModePaint);
                }
            };
            
            EditModeNone.OnSelected += _ => EditMode = PixelEditMode.None;
            EditModePaint.OnSelected += _ => EditMode = PixelEditMode.Paint;
            EditModeErase.OnSelected += _ => EditMode = PixelEditMode.Erase;
            EditModeColorPicker.OnSelected += _ => EditMode = PixelEditMode.ColorPicker;
            
            EditModeSelectGroup.SelectItem(EditModeNone);
        }

        void TemplatesPanelOnOnSelectChange(ItemGroup oldkey, ItemGroup selectedGroup)
        {
            if (selectedGroup)
            {
                SetupCanvas(selectedGroup[0] as PixelWeapon);
            }
            else
            {
                _editingImage = null;
                ResetLayout();
            }
        }

        /// <summary>
         /// Reload all UI elements with player inventory
         /// </summary>
        void ReloadUI()
        {
            // EditMode = PixelEditMode.None;
            PalettePanel.LoadInventory(Inventory, itemType => itemType is PixelType);
            TemplatesPanel.LoadInventory(Inventory, itemType => itemType is PixelWeaponType);
         
            TemplatesPanel.SelectIndex(0);
            
            // palette.InitPalette(pixels);
            // size = canvasSize;
            // UpdateLayout();
        }

        void SetupCanvas(PixelWeapon weapon)
        {
            _editingImage = weapon;
            _analyseWeapon = PixelWeapon.CreateFromPixelImage(
                weapon.Clone(false),
                weapon.HandelPosition,
                weapon.ForwardDirection);
            
            _weaponAnalyser = new PixelWeaponAnalyser(_analyseWeapon);
            this._undoStack.Clear();
            this._redoStack.Clear();
            ResetLayout();
            UpdateWeaponStats();
        }

        // public void SetEditable(Vector2Int pos, bool editable)
        // {
        //     if (!editable)
        //     {
        //         _lockedPixels.Add(pos);
        //         if (slots != null && slots.ContainsIndex(pos) && slots[pos.x, pos.y])
        //             slots[pos.x, pos.y].Editable = false;
        //     }
        //     else
        //     {
        //         _lockedPixels.Remove(pos);
        //         if (slots != null && slots.ContainsIndex(pos) && slots[pos.x, pos.y])
        //             slots[pos.x, pos.y].Editable = true;
        //     }
        // }

        public async Task Edit(Prototype.Inventory.Inventory inventory)
        {
            if (!(_promise is null))
            {
                if (!_undoStack.Any())
                {
                    _promise.SetResult(0);
                    _promise = null;
                    Hide();
                    return;
                }
                else
                {
                    Done();
                }
                
            }
                
            Inventory = inventory;
            // _editingImage = image;
            ReloadUI();

            Show();
            
            var promise = new TaskCompletionSource<int>();
            _promise = promise;

            var result = await promise.Task;
            _editingImage = null;
            Debug.Log("hide");

            Hide();
            
            // return result;
        }

        void ApplyImage(PixelImage target)
        {
            for(var y = 0 ;  y<CanvasSize.y;y++)
            for (var x = 0; x < CanvasSize.x; x++)
            {
                target.Pixels[x, y] = slots[x, y].Pixel;
            }
        }

        [EditorButton]
        void ResetLayout()
        {
            if (slots != null)
            {
                foreach (var slot in slots)
                {
                    GameObjectPool.Release(ResourceManager.Instance.PrefabPixelSlot, slot);
                }
            }
            slots = new PixelSlot[CanvasSize.x, CanvasSize.y];
            pixelCanvas.cellSize =
                MathUtility.Floor(((pixelCanvas.transform as RectTransform)?.rect.size ?? Vector2.one) / CanvasSize)
                - Vector2.one * GridLineWidth;
            for (var y = 0; y < CanvasSize.y; y++)
            {
                for (var x = 0; x < CanvasSize.x; x++)
                {
                    var slot = GameObjectPool.Get<PixelSlot>(ResourceManager.Instance.PrefabPixelSlot);
                    slot.transform.SetParent(pixelCanvas.transform, false);
                    slots[x, y] = slot;
                    slot.Setup(new Vector2Int(x, y));
                    slot.SetPixel(_editingImage.Pixels[x, y]);
                }
            }
        }

        internal void Undo()
        {
            if (this._undoStack.Count > 0)
            {
                var action = this._undoStack.Pop();
                switch (action.EditMode)
                {
                    case PixelEditMode.Paint: 
                        slots[action.Position.x, action.Position.y].SetPixel(action.OldPixel);
                        if (action.OldPixel)
                            Inventory.Take(action.OldPixel);
                        Inventory.SaveItem(action.NewPixel);
                        break;
                    case PixelEditMode.Erase:
                        slots[action.Position.x, action.Position.y].SetPixel(action.OldPixel);
                        if (action.OldPixel)
                            Inventory.Take(action.OldPixel);
                        break;
                }
                UpdateWeaponStats();
                _redoStack.Push(action);
            }
        }

        internal void Redo()
        {
            if (this._redoStack.Count > 0)
            {
                var action = this._redoStack.Pop();
                switch (action.EditMode)
                {
                    case PixelEditMode.Paint:
                        slots[action.Position.x, action.Position.y].SetPixel(action.NewPixel);
                        Inventory.Take(action.NewPixel);
                        Inventory.SaveItem(action.OldPixel);
                        break;
                    case PixelEditMode.Erase:
                        slots[action.Position.x, action.Position.y].SetPixel(null);
                        Inventory.SaveItem(action.OldPixel);
                        break;
                }
                UpdateWeaponStats();
                _undoStack.Push(action);
            }
        }

        internal void RecordAction(UndoAction action)
        {
            UpdateWeaponStats();
            this._undoStack.Push(action);
            this._redoStack.Clear();
        }

        internal void UpdateWeaponStats()
        {
            ApplyImage(_analyseWeapon);
            _weaponAnalyser.UpdateWeaponData();
            WeaponInfoLeft.text = "[Left Side]\r\n" +
                                  $"物理攻击力: +{_weaponAnalyser.LeftSideData.CommonDamage}\r\n" +
                                  $"水元素攻击力: +{_weaponAnalyser.LeftSideData.WaterDamage}\r\n" +
                                  $"土元素攻击力: +{_weaponAnalyser.LeftSideData.DirtDamage}\r\n" +
                                  $"火元素攻击力: +{_weaponAnalyser.LeftSideData.FlameDamage}\r\n" +
                                  $"风元素攻击力: +{_weaponAnalyser.LeftSideData.WindDamage}\r\n";
            WeaponInfoRight.text = "[Right Side]\r\n" +
                                   $"物理攻击力: +{_weaponAnalyser.RightSideData.CommonDamage}\r\n" +
                                   $"水元素攻击力: +{_weaponAnalyser.RightSideData.WaterDamage}\r\n" +
                                   $"土元素攻击力: +{_weaponAnalyser.RightSideData.DirtDamage}\r\n" +
                                   $"火元素攻击力: +{_weaponAnalyser.RightSideData.FlameDamage}\r\n" +
                                   $"风元素攻击力: +{_weaponAnalyser.RightSideData.WindDamage}\r\n";
            WeaponInfoSpike.text = "[Spike]\r\n" +
                                   $"物理攻击力: +{_weaponAnalyser.SpikeData.CommonDamage}\r\n" +
                                   $"水元素攻击力: +{_weaponAnalyser.SpikeData.WaterDamage}\r\n" +
                                   $"土元素攻击力: +{_weaponAnalyser.SpikeData.DirtDamage}\r\n" +
                                   $"火元素攻击力: +{_weaponAnalyser.SpikeData.FlameDamage}\r\n" +
                                   $"风元素攻击力: +{_weaponAnalyser.SpikeData.WindDamage}\r\n";
            WeaponInfoOverall.text = "[Overall]\r\n" +
                                     $"weight: {_weaponAnalyser.Weight}\r\n" +
                                     $"length: {_weaponAnalyser.Length}\r\n";
        }

        public async void Done()
        {
            var result = await Dialog.ShowPrefabAsync(
                UISettings.Current.BasicDialogPrefab,
                "Are you sure to save the pixel image?",
                this,
                DialogType.OkCancel);
            
            if (!result)
                return;
            
            ApplyImage(_analyseWeapon);
            _weaponAnalyser.UpdateWeaponData();
            if (_weaponAnalyser.BrokenPixels.Count > 0)
            {
                result = await Dialog.ShowPrefabAsync(
                    UISettings.Current.BasicDialogPrefab,
                    $"{_weaponAnalyser.BrokenPixels.Count} pixels not connected to weapon handle will be broken. " +
                    $"Are you sure to continue?",
                    this,
                    DialogType.OkCancel);
            
                if (!result)
                    return;
            }
            
            ApplyImage(_editingImage);
            
            _editingImage.UpdateTexture();
            _promise.SetResult(0);
            _promise = null;
        }

        public void ResetEditor()
        {
            
        }
    }

}