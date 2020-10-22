using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Rendering;
using Prototype.UI;
using Prototype.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prototype.Editor
{
    public class PixelEditorWindow : EditorWindow
    {
        private const int WorkSpaceHeight = 200;

        private const int PaletteMinHeight = 80;

        enum ShaderPass : int
        {
            RenderGrid = 0,
            RenderImage,
        }

        private Vector2Int _editSize = new Vector2Int(8, 8);
        private string _filePath;
        [SerializeField]
        private PixelImageAsset _editAsset;

        [SerializeField] private Texture2D _iconCursor;
        [SerializeField] private Texture2D _iconEraser;
        [SerializeField] private Texture2D _iconColorPicker;
        [SerializeField] private Texture2D _iconBrush;

        private PixelEditMode[] _modes = new[]
        {
            PixelEditMode.None,
            PixelEditMode.Paint,
            PixelEditMode.Erase,
            PixelEditMode.ColorPicker,
        };

        private Vector2 _paletteScrollPosition;

        private Dictionary<PixelType, Texture2D> _pixelTextures = new Dictionary<PixelType, Texture2D>();

        private PixelType _selectedPixelType;
        private PixelType _inspectingPixelType;

        private PixelEditMode _editMode = PixelEditMode.None;
        private RenderTexture _canvasGridRT;
        private RenderTexture _pixelImageRT;

        private PixelImage _editImage;
        private Mesh _imageMesh;

        [SerializeField]
        private GUISkin Skin;
        
        [MenuItem("Window/PixelEditor")]
        private static void ShowWindow()
        {
            var window = GetWindow();
            window.Show();
        }

        public static PixelEditorWindow GetWindow()
        {
            
            var window = GetWindow<PixelEditorWindow>();
            window.titleContent = new GUIContent("PixelEditor");
            window.NewImage();
            return window;
        }

        private void OnGUI()
        {
            var rect = position;
            rect.position = Vector2.zero;
            
            var canvasHeight = (int)position.height - WorkSpaceHeight;
            EditorGUI.DrawRect(new Rect(0, 0, position.width, canvasHeight), Color.gray);

            DrawEditor(canvasHeight);
            
            DrawWorkSpace(new Rect(0, canvasHeight, position.width, position.height - canvasHeight));
        }

        public void Edit(PixelImageAsset pixelImage)
        {
            if(!pixelImage)
                return;
            _editAsset = pixelImage;
            ReloadImage();
        }
        

        void DrawWorkSpace(Rect rect)
        {
            GUILayout.BeginArea(rect);
            
            var style = new GUIStyle();
            style.margin = new RectOffset(10, 10, 10, 10);

            var toolIdx = _modes.IndexOf(_editMode);
            toolIdx = GUILayout.SelectionGrid(toolIdx, new Texture[]
            {
                _iconCursor,
                _iconBrush,
                _iconEraser,
                _iconColorPicker
            }, 4);
            _editMode = _modes[toolIdx];
            
            
            EditorUtils.Verticle(style, () =>
            {
                _editSize = EditorGUILayout.Vector2IntField("Size", _editSize);
                _editAsset = EditorGUILayout.ObjectField("Asset", _editAsset, typeof(PixelImageAsset), true) as PixelImageAsset;
                
                EditorGUILayout.LabelField("Save Path");
                EditorUtils.Horizontal(() =>
                {
                    _filePath = EditorGUILayout.DelayedTextField(_filePath);
                    if (GUILayout.Button("Browse"))
                        _filePath = EditorUtility.SaveFilePanelInProject("Save", "PixelImage", "asset", "");
                    
                });
                EditorGUILayout.Space();
                
                EditorUtils.Horizontal(() =>
                {
                    if (GUILayout.Button("New"))
                        NewImage();
                    else if (GUILayout.Button("Reload"))
                        ReloadImage();
                    else if (GUILayout.Button("Save"))
                        SaveImage();
                });
            });
            
            
            GUILayout.EndArea();
        }

        void ReloadImage()
        {
            if (!_editAsset)
            {
                NewImage();
                return;
            }
            _editImage = new PixelImage(_editAsset.Size);
            for(var y = 0;y < _editAsset.Height; y++)
            for (var x = 0; x < _editAsset.Width; x++)
            {
                _editImage.Pixels[x, y] = _editAsset.Image.Pixels[x, y];
            }
            UpdateMeshData();
            RenderImage();
            Repaint();
        }

        void SaveImage()
        {
            _editAsset.Image = _editImage;
            var path = AssetDatabase.GetAssetPath(_editAsset);
            if(path == "")
                AssetDatabase.CreateAsset(_editAsset, _filePath);
            EditorUtility.SetDirty(_editAsset);
            ReloadImage();
        }

        void NewImage()
        {
            _editAsset = CreateInstance<PixelImageAsset>();
            _editAsset.name = "NewPixelImageAsset";
            _editImage = new PixelImage(_editSize);
            _editAsset.Image = _editImage;
            
            if (!_pixelImageRT)
            {
                _pixelImageRT = new RenderTexture(_editImage.Size.x * 64, _editImage.Size.y * 64, 0);
            }
            else
                _pixelImageRT.Release();

            _pixelImageRT.width = _editImage.Size.x * 64;
            _pixelImageRT.height = _editImage.Size.y * 64;
            _pixelImageRT.filterMode = FilterMode.Bilinear;
            _pixelImageRT.Create();

            _imageMesh = PixelImageEditorUtils.CreateImageMesh(_editImage);
            UpdateMeshData();
            RenderImage();
        }

        void UpdateMeshData()
            => PixelImageEditorUtils.UpdateImageMesh(_editImage, _imageMesh);

        void RenderImage()
            => PixelImageEditorUtils.RenderImage(_imageMesh, _pixelImageRT);

        void RenderCanvas()
        {
            var cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(_canvasGridRT);
            cmd.ClearRenderTarget(true, true, EditorUtils.HTMLColor("#444"));
            cmd.Blit(_pixelImageRT, _canvasGridRT, ShaderPool.Get("Prototype/Utils/BlitCopy"), 1);
            
            cmd.SetGlobalVector("RenderSize", new Vector4(
                _canvasGridRT.width, 
                    _canvasGridRT.height, 
                    1f / _canvasGridRT.width, 
                    1f / _canvasGridRT.height));
            cmd.SetGlobalVector("GridSize", new Vector2(_canvasGridRT.width, _canvasGridRT.height) / _editSize);
            cmd.SetGlobalVector("Color", Color.gray);
            cmd.Blit(BuiltinRenderTextureType.None, _canvasGridRT, ShaderPool.Get("Prototype/Editor/CanvasGridCode"), 0);
            
            Graphics.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        void DrawEditor(int height)
        {
            var canvasHeight = height - PaletteMinHeight;
            EditCanvas(new Rect(0, 0, position.width, canvasHeight));
            DrawPalette(new Rect(0, canvasHeight, position.width, PaletteMinHeight));
        }

        void DrawPalette(Rect rect)
        {
            GUIStyle style = new GUIStyle();
            GUILayout.BeginArea(rect);


            GUI.skin = Skin;
            EditorUtils.GridLayout(
                new Rect(Vector2.zero, rect.size - 15 * Vector2.one), 
                new Vector2(24, 24), 
                PixelAssetManager.Instance.PixelTypes,
                (elementRect, pixelType) =>
                {
                    if (DrawPaletteItem(elementRect, pixelType, pixelType == _selectedPixelType))
                    {
                        _selectedPixelType = pixelType;
                        _editMode = PixelEditMode.Paint;
                    }
                });
            GUI.skin = null;
            
            
            GUILayout.EndArea();
                
        }

        bool DrawPaletteItem(Rect rect, PixelType pixelType, bool selected)
        {
            var content = new GUIContent(GetOrCreateTexture(pixelType));
            var style = new GUIStyle();

            Skin.button.normal.background = GetOrCreateTexture(pixelType);
            Skin.button.active.background = GetOrCreateTexture(pixelType);
            var click = GUI.Button(rect, "", "button");
            if (selected)
                GUI.Toggle(rect, selected, "", "box");
            return click;
        }

        void EditCanvas(Rect rect)
        {
            EditorGUI.DrawRect(rect, EditorUtils.HTMLColor("#444"));

            int size = (int) Mathf.Min(rect.width, rect.height);
            if(!_canvasGridRT)
                _canvasGridRT = new RenderTexture(size, size, 0);
            if (size != _canvasGridRT.width * 2)
            {
                _canvasGridRT.Release();
                _canvasGridRT.width = _canvasGridRT.height = size * 2;
                _canvasGridRT.filterMode = FilterMode.Trilinear;
                _canvasGridRT.Create();
            }
            
            var drawRect = new Rect();
            drawRect.height = drawRect.width = size;
            drawRect.center = rect.center;

            if (Event.current.type == EventType.MouseDown)
            {
                var mousePos = Event.current.mousePosition - drawRect.min;
                var gridSize = drawRect.size / _editImage.Size;
                var gridPos = MathUtility.FloorToInt(mousePos / gridSize);
                gridPos.y = _editImage.Size.y - gridPos.y - 1;
                if (0 <= gridPos.x && gridPos.x < _editImage.Size.y && 0 <= gridPos.y && gridPos.y < _editImage.Size.y)
                    OnCanvasClick(gridPos);
            }

            // var cmd = CommandBufferPool.Get();
            // cmd.SetRenderTarget(_canvasGridRT);
            // cmd.ClearRenderTarget(true, true, Color.black);
            // cmd.SetGlobalVector("RenderSize", new Vector4(size, size, 1f / size, 1f / size));
            // cmd.SetGlobalVector("GridSize", size * Vector2.one / _editSize);
            // cmd.SetGlobalVector("Color", Color.gray);
            // cmd.Blit(BuiltinRenderTextureType.None, _canvasGridRT, ShaderPool.Get("Prototype/Editor/CanvasGridCode"), 0);
            // // cmd.DrawMesh(Utility.GetFullNDCQuad(), Matrix4x4.identity, ShaderPool.Get("Prototype/Editor/CanvasGrid"));
            // var fence = cmd.CreateAsyncGraphicsFence();
            // Graphics.ExecuteCommandBuffer(cmd);
            // cmd.Clear();
            // CommandBufferPool.Release(cmd);
            // // Graphics.WaitOnAsyncGraphicsFence(fence);
            // // GUI.DrawTexture(rect, _canvasRT, ScaleMode.ScaleToFit, false);
            // // Graphics.SetRenderTarget(_canvasRT);
            // // Graphics.Blit(null, _canvasRT, ShaderPool.Get("Prototype/Editor/EditorCanvas"), 0);
            //
            // // EditorGUI.DrawTextureAlpha(rect, _canvasRT);
            
            RenderCanvas();
            
            Graphics.DrawTexture(drawRect, _canvasGridRT);

        }

        void OnCanvasClick(Vector2Int gridPos)
        {
            if(_editImage is null)
                return;
            switch (_editMode)
            {
                case PixelEditMode.None:
                    InspectPixel(gridPos);
                    break;
                case PixelEditMode.Paint:
                    Paint(gridPos);
                    break;
                case PixelEditMode.Erase:
                    Erase(gridPos);
                    break;
                case PixelEditMode.ColorPicker:
                    PickPixel(gridPos);
                    break;
            }
        }

        void Paint(Vector2Int pos)
        {
            if (_selectedPixelType is null)
                return;

            _editImage.Pixels[pos.x, pos.y] = PixelAssetManager.CreatePixel(_selectedPixelType);
            UpdateMeshData();
            RenderImage();
            Repaint();
        }

        void Erase(Vector2Int pos)
        {
            _editImage.Pixels[pos.x, pos.y] = null;
            UpdateMeshData();
            RenderImage();
            Repaint();
        }
        

        void InspectPixel(Vector2Int pos)
        {
            _inspectingPixelType = _editImage?.Pixels[pos.x, pos.y]?.Type;
        }

        void PickPixel(Vector2Int pos)
        {
            _selectedPixelType = _editImage?.Pixels[pos.x, pos.y]?.Type;
            _inspectingPixelType = _selectedPixelType;
        }

        Texture2D GetOrCreateTexture(PixelType pixelType)
        {
            if (pixelType is null)
                return null;
            if (_pixelTextures.TryGetValue(pixelType, out var tex))
                return tex;
            var texture = new Texture2D((int)pixelType.sprite.rect.width, (int)pixelType.sprite.rect.height);
            texture.filterMode = FilterMode.Point;
            
            for (int y = 0; y < pixelType.sprite.rect.height; y++)
            for (int x = 0; x < pixelType.sprite.rect.width; x++)
            {
                var color = pixelType.sprite.texture.GetPixel(
                    x + (int) pixelType.sprite.rect.xMin,
                    y + (int) pixelType.sprite.rect.yMin);
                texture.SetPixel(x, y, color);
            }

            texture.Apply();
            _pixelTextures[pixelType] = texture;
            return texture;

        }
        
        
        
    }
}