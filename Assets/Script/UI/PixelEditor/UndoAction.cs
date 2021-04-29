using Prototype.Element;
using UnityEngine;

namespace Prototype.UI
{
    public struct UndoAction
    {
        public Vector2Int Position;
        public PixelEditMode EditMode;
        public Pixel NewPixel;
        public Pixel OldPixel;
    }
}