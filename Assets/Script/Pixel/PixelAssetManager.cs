using System.Collections.Generic;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Element
{
    [ExecuteInEditMode]
    public class PixelAssetManager : Singleton<PixelAssetManager>
    {
        public Material PixelImageMaterial;
        public Texture2D PixelTexture;
        public List<PixelType> PixelTypes = new List<PixelType>();

        public static PixelType FindPixelType(string name)
        {
            foreach (var type in Instance.PixelTypes)
            {
                if (type.name == name)
                    return type;
            }

            return null;
        }

        public static Pixel CreatePixel(PixelType type)
        {
            if (!type)
                return null;
            return new Pixel(type);
        }
        
    }
}