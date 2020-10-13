using System.Collections.Generic;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Element
{
    public class PixelAssetManager : Singleton<PixelAssetManager>
    {
        public Material PixelImageMaterial;
        public Texture2D PixelTexture;
        public List<PixelType> PixelTypes = new List<PixelType>();
    }
}