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
        /// <summary>
        /// Hardness drop rate per hit
        /// </summary>
        public float WearRate;

        /// <summary>
        /// Damage per hit
        /// </summary>
        public float Damage => (Pixel?.Damage ?? 0) * DamageRate;
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
            /// <summary>
            /// (x, y, pixelX, pixelY) -> bool (should ignore)
            /// </summary>
            public Func<int, int, int, int, bool> IgnorePixel;

            public void Reset()
            {
                for (var x = 0; x < DamageAttenuationField.GetLength(0); x++)
                for (var y = 0; y < DamageAttenuationField.GetLength(1); y++)
                {
                    DamageAttenuationField[x, y] = -1;
                }

                foreach (var i in OccludedPixels.Indices)
                    OccludedPixels[i] = OcclusionClearValue;

            }
        }
        
        public readonly PixelImage Image;
        public readonly WeaponPixelData[,] WeaponDataLeft;
        public readonly WeaponPixelData[,] WeaponDataRight;
        public readonly WeaponForwardDirection ForwardCorner;


        public readonly OneSideAnalyserHelper LeftAnalyser;
        public readonly OneSideAnalyserHelper RightAnalyser;
        
        public float _paramP = 0.57f;
        public float _paramK = 1.02f;
        public float _paramY = 1.9f;
        public float _paramE = 2.52f;

        public float[] _attenuationLevels = new [] {-1, 0.01f, 0.9f, 3.6f, 7.52f, 18.62f};

        private Vector2Int _gridOrigin;
        public Vector2 Origin { get; private set; }
        public float Length { get; private set; }
        public float Mass { get; private set; }
        public float Inertia { get; private set; }
        
        public float TotalDamageLeft { get; private set; }
        public float TotalDamageRight { get; private set; }

        public PixelWeaponAnalyser(PixelImage image, WeaponForwardDirection forwardCorner)
        {
            Image = image;
            WeaponDataLeft = new WeaponPixelData[image.Size.x, image.Size.y];
            WeaponDataRight = new WeaponPixelData[image.Size.x, image.Size.y];
            ForwardCorner = forwardCorner;

            switch (forwardCorner)
            {
                case WeaponForwardDirection.TopLeft:
                    _gridOrigin = new Vector2Int(image.Size.x, 0);
                    Origin = new Vector2(.5f, -.5f);
                    
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
                        DamageAttenuationField = new float[image.Size.x, image.Size.y],
                        IgnorePixel = (x, y, pixelX, pixelY) => (x + y) < (pixelX + pixelY)
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
                        DamageAttenuationField = new float[image.Size.x, image.Size.y],
                        IgnorePixel = (x, y, pixelX, pixelY) => (x + y) > (pixelX + pixelY),
                    };
                    
                    
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

            GenerateWeaponData();
            
            PhysicalAnalyse();
        }

        void PhysicalAnalyse()
        {

            Mass = 0;
            Inertia = 0;
            Length = 0;

            for (var y = 0; y < Image.Size.y; y++)
            for (var x = 0; x < Image.Size.x; x++)
            {
                if(Image[x, y] is null)
                    continue;

                var r = Vector2.Distance(new Vector2(x, y), _gridOrigin);
                Length = Mathf.Max(Length, r);
                Inertia += Image[x, y].Weight * r * r;
                Mass += Image[x, y].Weight;
            }
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

                for (var y = 0; y < Image.Size.y; y++) 
                for (var x = 0; x < Image.Size.x; x++)
                {
                    if(x == posX && y == posY)
                        continue;
                    if(helper.IgnorePixel(x, y, posX, posY))
                        continue;
                
                    var delta = new Vector2(x, y) - new Vector2(posX, posY);
                    
                    var attenuation = PixelAttenuation(delta, helper.Tangent, -helper.Normal);
                    helper.DamageAttenuationField[x, y] = Mathf.Max(helper.DamageAttenuationField[x, y], attenuation);
                }
            }
        }

        /// <summary>
        /// For given pixel (x, y) return whether it is occluded by other pixels. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="topMostPixel"></param>
        /// <param name="compareMode"></param>
        /// <returns></returns>
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
            var z = 2 * p * delta.y - delta.x * delta.x;
            if (z < 0)
                return 0;
            else
            {
                z = Mathf.Sqrt(z);
                z = Mathf.Pow(z, _paramE) * _paramK;
                return z;
            }
        }

        void GenerateWeaponData()
        {
            TotalDamageLeft = 0;
            TotalDamageRight = 0;
            for (var y = 0; y < Image.Size.y; y++)
            for (var x = 0; x < Image.Size.x; x++)
            {
                if (Image[x, y] is null)
                {
                    WeaponDataLeft[x, y] = WeaponDataRight[x, y] = new WeaponPixelData()
                    {
                        Pixel = null,
                        DamageRate = 0,
                        WearRate = 0
                    };
                    continue;
                }
                var r = Vector2.Distance(new Vector2(x, y), _gridOrigin);
                WeaponDataLeft[x, y] = GenerateWeaponData(x, y, r, LeftAnalyser.DamageAttenuationField[x, y]);
                WeaponDataRight[x, y] = GenerateWeaponData(x, y, r, RightAnalyser.DamageAttenuationField[x, y]);

                TotalDamageLeft += WeaponDataLeft[x, y].Damage;
                TotalDamageRight += WeaponDataRight[x, y].Damage;
            }
        }

        WeaponPixelData GenerateWeaponData(int x, int y, float distance, float damageAttenuation)
        {
            // will never get -1 here, since it is only for null pixels.
            var attenuationLevel = MathUtility.ListRangeMap(
                _attenuationLevels,
                new[] {-1, 0, 1, 2, 3, 4}, damageAttenuation);

            var attenuation = 1.0f / (1 + attenuationLevel);

            var pixel = Image[x, y];
            // TODO: Complete weapon data.
            return new WeaponPixelData()
            {
                Pixel = pixel,
                DamageRate = attenuation,
                WearRate = attenuation,
            };
        }
        
        

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