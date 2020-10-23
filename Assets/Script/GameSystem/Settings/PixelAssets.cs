using System;
using System.Collections.Generic;
using Prototype.Element;
using UnityEngine;

namespace Prototype.Settings
{
    [Serializable]
    public class PixelAssets : SettingEntry<PixelAssets>
    {
        public Material PixelMaterial;
        public Texture PixelTexture;
        public List<PixelType> PixelTypes = new List<PixelType>();
    }
}