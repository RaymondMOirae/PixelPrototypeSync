using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Element
{
    public enum WeaponForwardDirection
    {
        TopLeft = Utility.RectCorner.XMinYMax,
        TopRight = Utility.RectCorner.XMaxYMax,
        BottomLeft = Utility.RectCorner.XMinYMin,
        BottomRight = Utility.RectCorner.XMaxYMin,
    }
    public struct WeaponPixelData
    {
        public Pixel Pixel;
        public float DamageRate;
        public float WearRate;
        
    }
    public class PixelWeaponAnalyser
    {
        enum PixelCompareMode : int
        {
            Less = -1,
            Greater = 1,
        }

        public class OneSideAnalyserHelper
        {
            public Vector2 Tangent;
            public Vector2 Normal;
            public int OcclusionClearValue;
            public IEnumerable<(int x, int y)> PixelIterator;
            public Func<int, int, bool> OcclusionChecker;
            public float[,] DamageAttenuationField;
            public Utility.OffsetArray<int> OccludedPixels;

            public void Reset()
            {
                for (var x = 0; x < DamageAttenuationField.GetLength(0); x++)
                for (var y = 0; y < DamageAttenuationField.GetLength(1); y++)
                {
                    DamageAttenuationField[x, y] = 0;
                }

                foreach (var i in OccludedPixels.Indices)
                    OccludedPixels[i] = OcclusionClearValue;

            }
        }
        
        public readonly PixelImage Image;
        public readonly WeaponPixelData[,] WeaponDataLeft;
        public readonly WeaponPixelData[,] WeaponDataRight;
        public readonly WeaponForwardDirection ForwardCorner;
        // public readonly WeaponForwardDirection RightCorner;
        // public readonly WeaponForwardDirection LeftCorner;
        // public float[,] _damageLeftAttenuationField;
        // public float[,] _damageRightAttenuationField;
        // private Vector2 _normalDirection;
        // private Vector2 _tangentDirection;

        /*
         * In diagonal coordinate
         * use ToDiagonal() & FromDiagonal() transform between normal coordinate & diagonal coordinate
         *            y
         *    \ | | / 
         * ----\--/---
         *     |X|   
         * ---/--\---
         *  / | | \
         *         x
         */
        // private readonly Utility.OffsetArray<int> _leftMostPixel;
        // private readonly Utility.OffsetArray<int> _rightMostPixel;

        public readonly OneSideAnalyserHelper LeftAnalyser;
        public readonly OneSideAnalyserHelper RightAnalyser;
        
        public float _paramP = 0.57f;
        public float _paramK = 1.02f;
        public float _paramY = 1.9f;
        public float _paramE = 2.52f;

        public PixelWeaponAnalyser(PixelImage image, WeaponForwardDirection forwardCorner)
        {
            Image = image;
            WeaponDataLeft = new WeaponPixelData[image.Size.x, image.Size.y];
            WeaponDataRight = new WeaponPixelData[image.Size.x, image.Size.y];
            // _damageLeftAttenuationField = new float[image.Size.x, image.Size.y];
            // _damageRightAttenuationField = new float[image.Size.x, image.Size.y];
            ForwardCorner = forwardCorner;

            switch (forwardCorner)
            {
                case WeaponForwardDirection.TopLeft:
                    LeftAnalyser = new OneSideAnalyserHelper()
                    {
                        Tangent = new Vector2(-1, 1).normalized,
                        Normal = new Vector2(-1, -1).normalized,
                        OcclusionClearValue = int.MaxValue,
                        OccludedPixels =
                            new Utility.OffsetArray<int>(-image.Size.y - 3, image.Size.x + image.Size.y + 4),
                        PixelIterator =
                            Utility.DiagonalIndices(image.Size.x, image.Size.y, Utility.RectCorner.XMinYMin),
                        OcclusionChecker = (x, y) =>
                            PixelOccluded(x, y, LeftAnalyser.OccludedPixels, PixelCompareMode.Less),
                        DamageAttenuationField = new float[image.Size.x, image.Size.y]
                    };

                    RightAnalyser = new OneSideAnalyserHelper()
                    {
                        Tangent = new Vector2(-1, 1).normalized,
                        Normal = new Vector2(1, 1).normalized,
                        OcclusionClearValue = int.MinValue,
                        OccludedPixels =
                            new Utility.OffsetArray<int>(-image.Size.y - 3, image.Size.x + image.Size.y + 4),
                        PixelIterator =
                            Utility.DiagonalIndices(image.Size.x, image.Size.y, Utility.RectCorner.XMaxYMax),
                        OcclusionChecker = (x, y) =>
                            PixelOccluded(x, y, RightAnalyser.OccludedPixels, PixelCompareMode.Greater),
                        DamageAttenuationField = new float[image.Size.x, image.Size.y]
                    };
                    
                    // _normalDirection = new Vector2(1, 1).normalized;
                    // _tangentDirection = new Vector2(-1, 1).normalized;
                    // _leftMostPixel = new Utility.OffsetArray<int>(-image.Size.y - 3, image.Size.x + image.Size.y + 4);
                    // _rightMostPixel = new Utility.OffsetArray<int>(_leftMostPixel.StartIndex, _leftMostPixel.Count);
                    // RightCorner = WeaponForwardDirection.TopRight;
                    // LeftCorner = WeaponForwardDirection.BottomLeft;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void UpdateWeaponData()
        {
            
            for(var y = 0;y<Image.Size.y; y++)
            for (var x = 0; x < Image.Size.x; x++)
            {
                WeaponDataLeft[x, y] = new WeaponPixelData()
                {
                    Pixel = Image[x, y],
                    DamageRate = 1,
                    WearRate = 1,
                };
                WeaponDataRight[x, y] = new WeaponPixelData()
                {
                    Pixel = Image[x, y],
                    DamageRate = 1,
                    WearRate = 1,
                };
            }
            
            LeftAnalyser.Reset();
            RightAnalyser.Reset();
            
            AnalyseOneSide(LeftAnalyser);
            AnalyseOneSide(RightAnalyser);
            
        }

        void AnalyseOneSide(OneSideAnalyserHelper helper)
        {
            foreach (var (posX, posY) in helper.PixelIterator)
            {
                var diagonalPos = ToDiagonal(new Vector2Int(posX, posY));

                var occluded = helper.OcclusionChecker(diagonalPos.x, diagonalPos.y);
                if (occluded)
                {
                    helper.DamageAttenuationField[posX, posY] = float.MaxValue;
                    continue;
                    
                }

                if (Image[posX, posY] is null)
                    continue;

                helper.OccludedPixels[diagonalPos.x] = diagonalPos.y;
                
                for(var y = 0;y<Image.Size.y; y++)
                for (var x = 0; x < Image.Size.x; x++)
                {
                    if(x == posX && y == posY)
                        continue;
                    if(x + y > posX + posY)
                        continue;
                
                    var delta = new Vector2(x, y) - new Vector2(posX, posY);
                    
                    var attenuation = PixelAttenuation(delta, helper.Tangent, -helper.Normal);
                    helper.DamageAttenuationField[x, y] = Mathf.Max(helper.DamageAttenuationField[x, y], attenuation);
                }
            }
        }

        bool PixelOccluded(int x, int y, IList<int> topMostPixel, PixelCompareMode compareMode)
        {
            var leftOccluded = topMostPixel[x - 1].CompareTo(y) == (int)compareMode ||
                               (topMostPixel[x].CompareTo(y) == (int)compareMode && topMostPixel[x - 2].CompareTo(y) == (int)compareMode);
            var rightOccluded = topMostPixel[x + 1].CompareTo(y) == (int)compareMode ||
                                (topMostPixel[x].CompareTo(y) == (int)compareMode && topMostPixel[x + 2].CompareTo(y) == (int)compareMode);
            return leftOccluded && rightOccluded;
        }

        float PixelAttenuation(Vector2 delta, Vector2 tangentVector, Vector2 normalVector)
        {
            delta = new Vector2(Vector2.Dot(delta, tangentVector), Vector2.Dot(delta, normalVector));
            delta.y += _paramY;
            
            var p = _paramP;
            if (2 * p * delta.y - delta.x * delta.x < 0)
                return 0;
            else
            {
                var z = Mathf.Sqrt(2 * p * delta.y - delta.x * delta.x);
                z = Mathf.Pow(z, _paramE) * _paramK;
                return z;
            }
        }

        public static Vector2Int ToDiagonal(Vector2Int v)
        {
            return new Vector2Int(v.x - v.y, v.x + v.y);
        }

        public static Vector2Int FromDiagonal(Vector2Int v)
        {
            return new Vector2Int((v.x + v.y) / 2, (v.y - v.x) / 2);
        }
    }
}