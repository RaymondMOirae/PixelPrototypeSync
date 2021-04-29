using System;
using Prototype.Inventory;
using UnityEngine;

namespace Prototype.Element
{
    [Serializable]
    public class Pixel : Item
    {
        [SerializeField]
        private PixelType _type;
        public PixelType Type => _type;
        public float Endurance;

        /// <summary>
        /// Indicate whether this pixel can be modified or destroyed from PixelImage.
        /// </summary>
        public bool Protected = false;

        public float Weight => _type ? _type.weight : 0;
        public float Damage => _type ? _type.hardness : 0;

        public Pixel(PixelType type)
        {
            this._type = type;
            this.Endurance = type.endurance;
        }

        public static implicit operator bool(Pixel pixel)
            => !(pixel?._type is null);

        public override ItemType ItemType => _type;
    }
}