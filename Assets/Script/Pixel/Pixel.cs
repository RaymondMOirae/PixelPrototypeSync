using System;
using UnityEngine;

namespace Prototype.Element
{
    [Serializable]
    public class Pixel
    {
        [SerializeField]
        private PixelType _type;
        public PixelType Type => _type;
        public float Hardness;

        public Pixel(PixelType type)
        {
            this._type = type;
            this.Hardness = type.hardness;
        }
    }
}