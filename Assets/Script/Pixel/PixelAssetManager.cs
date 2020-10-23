using System.Collections.Generic;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Element
{
    public class PixelAssetManager : Singleton<PixelAssetManager>
    {

        public static PixelType FindPixelType(string name)
        {
            foreach (var type in PixelAssets.Current.PixelTypes)
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

        protected override void Awake()
        {
            base.Awake();  
        }
    }
}