using System;
using Prototype.Element;
using UnityEditor;
using UnityEngine;

namespace Prototype.Editor
{
    [CustomEditor(typeof(PixelImageAsset))]
    public class EditorPixelImage : UnityEditor.Editor
    {
        private Vector2Int _size;

        private void Awake()
        {
            var image = target as PixelImageAsset;
            _size = image.Size;
        }

        public override void OnInspectorGUI()
        {
            var image = target as PixelImageAsset;
            _size = EditorGUILayout.Vector2IntField("Size", _size);
            EditorGUILayout.Space();
            if (GUILayout.Button("Resize"))
            {
                image.Resize(_size);
                EditorUtility.SetDirty(image);
            }
        }
    }

    [CustomPreview(typeof(PixelImageAsset))]
    public class PixelImagePreview : UnityEditor.ObjectPreview
    {
        private RenderTexture _imagePreviewTexture;
        private Mesh _previewMesh;
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            var image = target as PixelImageAsset;

            if (!_imagePreviewTexture)
            {
                _imagePreviewTexture = new RenderTexture(image.Width * 8, image.Height * 8, 0);
                _imagePreviewTexture.filterMode = FilterMode.Point;
                _imagePreviewTexture.Create();
            }

            if (!_previewMesh)
                _previewMesh = PixelImageEditorUtils.CreateImageMesh(image.Image);
            
            PixelImageEditorUtils.UpdateImageMesh(image.Image, _previewMesh);
            PixelImageEditorUtils.RenderImage(_previewMesh, _imagePreviewTexture);

            var rect = r;
            rect.size = Mathf.Min(r.width, r.height) * Vector2.one;
            rect.center = r.center;
            GUI.DrawTexture(r, _imagePreviewTexture, ScaleMode.ScaleToFit, true);
        }
    }
}