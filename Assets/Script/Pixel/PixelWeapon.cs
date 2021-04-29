using UnityEngine;

namespace Prototype.Element
{
    public class PixelWeapon : PixelImage
    {
        public Vector2Int HandelPosition { get; }
        public WeaponForwardDirection ForwardDirection { get; }
        public PixelWeapon(Vector2Int size, Vector2Int handelPosition, WeaponForwardDirection forwardDirection) : base(size)
        {
            HandelPosition = handelPosition;
            ForwardDirection = forwardDirection;
        }

        public static PixelWeapon CreateFromPixelImage(PixelImage pixelImage, Vector2Int handelPosition,
            WeaponForwardDirection forwardDirection)
        {
            var weapon = new PixelWeapon(pixelImage.Size, handelPosition, forwardDirection);
            for(var y = 0;y < pixelImage.Size.y;y++)
            for (var x = 0; x < pixelImage.Size.x; x++)
            {
                weapon.Pixels[x, y] = pixelImage.Pixels[x, y];
            }

            weapon.UpdateTexture();
            return weapon;
        }
    }
}