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

        public float Weight => _type ? _type.weight : 0;
        public float Damage => _type ? _type.damage : 0;

        public Pixel(PixelType type)
        {
            this._type = type;
            this.Hardness = type.hardness;
        }
    }
}