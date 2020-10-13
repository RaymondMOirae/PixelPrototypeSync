using Prototype.Element;
using UnityEditor;
using UnityEngine;

namespace Prototype.Editor
{
    [CustomPreview(typeof(PixelType))]
    public class EditorPixelPreview : ObjectPreview
    {
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (target is PixelType pixel && pixel.sprite)
            {
                var center = r.center;
                r.size = Vector2.one * Mathf.Min(r.size.x, r.size.y);
                r.center = center;
                GUI.DrawTextureWithTexCoords(r, pixel.sprite.texture,  pixel.sprite.rect.Scale(pixel.sprite.texture.texelSize), false);
            }
        }
    }
}