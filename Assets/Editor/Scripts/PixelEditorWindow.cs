using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Rendering;
using Prototype.Settings;
using Prototype.UI;
using Prototype.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prototype.Editor
{
    public class PixelEditorWindow : EditorWindow
    {
        private const int WorkSpaceHeight = 500;

        private const int PaletteMinHeight = 80;

        enum DebugLayer
        {
            None,
            LeftAttenuation,
            RightAttenuation,
            StabAttenuation,
        }

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
        [SerializeField] private Texture2D _iconArrowBottomLeft;
        [SerializeField] private Texture2D _iconArrowTopRight;
        [SerializeField] private Texture2D _iconArrowTopLeft;
        [SerializeField] private Texture2D _iconHide;

        private PixelEditMode[] _modes = new[]
        {
            PixelEditMode.None,
            PixelEditMode.Paint,
            PixelEditMode.Erase,
            PixelEditMode.ColorPicker,
        };
        private PixelEditMode _currentEditMode = PixelEditMode.None;

        private DebugLayer[] _debugLayer = new[]
        {
            DebugLayer.None,
            DebugLayer.LeftAttenuation,
            DebugLayer.StabAttenuation,
            DebugLayer.RightAttenuation,
        };

        private DebugLayer _currentDebugLayer = DebugLayer.None;

        private Vector2 _paletteScrollPosition;

        private Dictionary<PixelType, Texture2D> _pixelTextures = new Dictionary<PixelType, Texture2D>();

        private PixelType _selectedPixelType;
        private PixelType _inspectingPixelType;

        private RenderTexture _canvasGridRT;
        private RenderTexture _pixelImageRT;

        private PixelWeapon _editImage;
        private Mesh _imageMesh;
        private Mesh _analyseMesh;
        private PixelWeaponAnalyser _analyser;

        private bool _showParametersEditor = true;

        private float _analyseParamP = 0.48f;
        private float _analyseParamK = 1.02f;
        private float _analyseParamY = 2.26f;
        private float _analyseParamE = 2.73f;

        private float[] _thresholds = new float[4] {0.9f, 3.6f, 7.52f, 18.62f};

        private PixelWeaponAnalyser.OneSideAnalyserHelper _analyserHelper
        {
            get
            {
                switch (_currentDebugLayer)
                {
                    case DebugLayer.LeftAttenuation:
                        return _analyser.LeftAnalyser;
                    case DebugLayer.RightAttenuation:
                        return _analyser.RightAnalyser;
                    case DebugLayer.StabAttenuation:
                        return _analyser.StabAnalyser;
                    default:
                        return null;
                }
            }
        }

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
            if(!_editImage || _analyser is null)
                NewImage();
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

        void DrawToolBar(Rect rect)
        {
            
        }

        void DrawInfoArea(Rect rect)
        {
            
        }
        

        void DrawWorkSpace(Rect rect)
        {
            rect = EditorUtils.HorizontalSplit(rect, 56, (area) =>
            {
                var style = new GUIStyle();
                style.padding = new RectOffset(10, 10, 10, 10);
                EditorUtils.Area(area, style, () =>
                {
                    EditorUtils.Horizontal(() =>
                    {
                        var toolStyle = Skin.toggle;
                        var toolIdx = _modes.IndexOf(_currentEditMode);
                        toolIdx = GUILayout.SelectionGrid(toolIdx, new Texture[]
                        {
                            _iconCursor,
                            _iconBrush,
                            _iconEraser,
                            _iconColorPicker
                        }, 4, toolStyle);
                        _currentEditMode = _modes[toolIdx];
                        
                        EditorGUILayout.Space();

                        var debugLayerIdx = _debugLayer.IndexOf(_currentDebugLayer);
                        debugLayerIdx = GUILayout.SelectionGrid(debugLayerIdx, new Texture[]
                        {
                            _iconHide,
                            _iconArrowBottomLeft,
                            _iconArrowTopLeft,
                            _iconArrowTopRight,
                        }, 4, toolStyle);
                        _currentDebugLayer = _debugLayer[debugLayerIdx];

                    });
                });
            });
            rect = EditorUtils.HorizontalSplit(rect, 100, (area) =>
            {
                
                EditorGUI.DrawRect(area.Shrink(10), EditorUtils.HTMLColor("#E4E4E4"));
                EditorUtils.Area(area.Shrink(20) , () =>
                {
                    if(_analyser is null)
                        return;
                    EditorUtils.Horizontal(() =>
                    {
                        EditorUtils.Verticle(() =>
                        {
                            EditorGUILayout.LabelField("Mass", _analyser.Weight.ToString("F1"));
                            EditorGUILayout.LabelField("Intertia", _analyser.Inertia.ToString("F1"));
                            EditorGUILayout.LabelField("Length", _analyser.Length.ToString("F1"));
                        });
                        EditorUtils.Verticle(() =>
                        {
                            EditorGUILayout.LabelField("Left Damage", _analyser.TotalDamageLeft.ToString("F1"));
                            EditorGUILayout.LabelField("Right Damage", _analyser.TotalDamageRight.ToString("F1"));
                            EditorGUILayout.LabelField("Stab Damage", _analyser.TotalDamageStab.ToString("F1"));
                        });
                    });
                });
            });
            
            EditorUtils.Area(rect, () =>
            {
                EditorGUILayout.Space();
            
                var style = new GUIStyle();
                style.margin = new RectOffset(10, 10, 0, 10);
                EditorUtils.Verticle(style, () =>
                {
                    if (_analyserHelper != null)
                    {
                        EditorUtils.Fold("Parameters", ref _showParametersEditor, () =>
                        {
                            if (_analyserHelper is null)
                            {
                                _analyseParamP = EditorGUILayout.FloatField("p", _analyseParamP);
                                _analyseParamK = EditorGUILayout.FloatField("k", _analyseParamK);
                                _analyseParamY = EditorGUILayout.FloatField("y", _analyseParamY);
                                _analyseParamE = EditorGUILayout.FloatField("e", _analyseParamE);
                            }
                            else
                            {
                                _analyserHelper.AttenuationFieldParams.p = EditorGUILayout.FloatField("p", _analyserHelper.AttenuationFieldParams.p);
                                _analyserHelper.AttenuationFieldParams.k = EditorGUILayout.FloatField("k", _analyserHelper.AttenuationFieldParams.k);
                                _analyserHelper.AttenuationFieldParams.y = EditorGUILayout.FloatField("y", _analyserHelper.AttenuationFieldParams.y);
                                _analyserHelper.AttenuationFieldParams.e = EditorGUILayout.FloatField("e", _analyserHelper.AttenuationFieldParams.e);
                            }

                            EditorGUILayout.LabelField("Threshold");
                            for (var i = 2; i < _analyser._attenuationLevels.Length; i++)
                                _analyser._attenuationLevels[i] = EditorGUILayout.FloatField(i.ToString(), _analyser._attenuationLevels[i]);
                        
                            UpdateMeshData();
                        });
                    }
                    
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
            });
        }

        void ReloadImage()
        {
            if (!_editAsset)
            {
                NewImage();
                return;
            }

            _editImage = new PixelWeapon(_editAsset.Size, new Vector2Int(_editAsset.Size.x - 1, 0),
                WeaponForwardDirection.TopLeft);
            
            for(var y = 0;y < _editAsset.Height; y++)
            for (var x = 0; x < _editAsset.Width; x++)
            {
                _editImage.Pixels[x, y] =
                    _editAsset.Image.Pixels[x, y] ? new Pixel(_editAsset.Image.Pixels[x, y].Type) : null;
            }
            _analyser = new PixelWeaponAnalyser(_editImage);
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
            _editImage = new PixelWeapon(_editSize, new Vector2Int(_editSize.x - 1, 0), WeaponForwardDirection.TopLeft);
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
            _analyseMesh = PixelImageEditorUtils.CreateImageMesh(_editImage);
            
            _analyser = new PixelWeaponAnalyser(_editImage);
            
            UpdateMeshData();
            RenderImage();
        }

        void UpdateMeshData()
        {
            if (!_editImage)
                return;
            PixelImageEditorUtils.UpdateImageMesh(_editImage, _imageMesh);
            ReloadAnalyse(1);
        }

        void RenderImage()
            => PixelImageEditorUtils.RenderImage(_imageMesh, _pixelImageRT);

        void RenderDebugLayer(CommandBuffer cmd)
        {
            if(_currentDebugLayer == DebugLayer.None)
                return;
            
            if(!_analyseMesh)
                return;

            cmd.DrawMesh(_analyseMesh, Matrix4x4.identity, ShaderPool.Get("Prototype/Editor/CanvasGridCode"), 0, 2);
        }

        void RenderCanvas()
        {
            var cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(_canvasGridRT);
            cmd.ClearRenderTarget(true, true, EditorUtils.HTMLColor("#444"));
            cmd.Blit(_pixelImageRT, _canvasGridRT, ShaderPool.Get("Prototype/Utils/BlitCopy"), 1);

            RenderDebugLayer(cmd);
            
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

        void ReloadAnalyse(int direction)
        {
            // Reset debug layer mesh colors to transparent.
            var colors = _analyseMesh.colors;
            for(var y = 0; y < _editSize.y;y ++)
            for (var x = 0; x < _editSize.x; x++)
            {
                var baseIdx = y * _editSize.x * 4 + x * 4;
                colors[baseIdx + 0]
                    = colors[baseIdx + 1]
                        = colors[baseIdx + 2]
                            = colors[baseIdx + 3] = Color.white.Transparent();
            }

            // _analyser._paramY = _analyseParamY;
            // _analyser._paramE = _analyseParamE;
            // _analyser._paramK = _analyseParamK;
            // _analyser._paramP = _analyseParamP;
            _thresholds[0] = _analyser._attenuationLevels[2];
            _thresholds[1] = _analyser._attenuationLevels[3];
            _thresholds[2] = _analyser._attenuationLevels[4];
            _thresholds[3] = _analyser._attenuationLevels[5];
            _analyser.UpdateWeaponData();

            for(var y = 0; y < _editSize.y;y ++)
            for (var x = 0; x < _editSize.x; x++)
            {
                var baseIdx = y * _editSize.x * 4 + x * 4;
                float rawValue = 0;
                switch (_currentDebugLayer)
                {
                    case DebugLayer.LeftAttenuation:
                        rawValue = _analyser.LeftAnalyser.DamageAttenuationField[x, y];
                        break;
                    case DebugLayer.RightAttenuation:
                        rawValue = _analyser.RightAnalyser.DamageAttenuationField[x, y];
                        break;
                    case DebugLayer.StabAttenuation:
                        rawValue = _analyser.StabAnalyser.DamageAttenuationField[x, y];
                        break;
                }
                if(rawValue <= 0)
                    continue;
                var value = rawValue;
                // var value = MathUtility.RangeMapClamped(0, maxValue, 0, 1, _analyser.RightAnalyser.DamageAttenuationField[x, y]);
                value = MathUtility.ListRangeMap(
                    new[] {0, _thresholds[0], _thresholds[1], _thresholds[2], _thresholds[3]},
                    new[] {0, 1, 2, 3, 4}, value);
                colors[baseIdx + 0]
                    = colors[baseIdx + 1]
                    = colors[baseIdx + 2]
                    = colors[baseIdx + 3] 
                    = Color.HSVToRGB((1 - (value - 1) / 3) / 2, 1, 1);
                    
            }
         
            _analyseMesh.SetColors(colors);
            _analyseMesh.UploadMeshData(false); 
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
                PixelAssets.Current.PixelTypes,
                (elementRect, pixelType) =>
                {
                    if (DrawPaletteItem(elementRect, pixelType, pixelType == _selectedPixelType))
                    {
                        _selectedPixelType = pixelType;
                        _currentEditMode = PixelEditMode.Paint;
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
            switch (_currentEditMode)
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