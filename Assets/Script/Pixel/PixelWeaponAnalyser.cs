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
        public enum PixelCompareMode : int
        {
            Less = -1,
            Greater = 1,
        }

        public struct AttenuationFieldParameters
        {
            public float p;
            public float k;
            public float y;
            public float e;
        }

        public class OneSideAnalyserHelper
        {
            public Vector2 Tangent;
            public Vector2 Normal;
            public int OcclusionClearValue;
            public IEnumerable<(int x, int y)> PixelIterator;
            // public Func<int, int, bool> OcclusionChecker;
            public Func<int, int, (int, int)> OcclusionCoordMap;
            public PixelCompareMode OcclusionCompareMode;
            
            public float[,] DamageAttenuationField;
            public Utility.OffsetArray<int> OccludedPixels;
            /// <summary>
            /// (x, y, pixelX, pixelY) -> bool (should ignore)
            /// </summary>
            public Func<int, int, int, int, bool> IgnorePixel;

            public AttenuationFieldParameters AttenuationFieldParams;

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
        
        public readonly PixelWeapon Weapon;
        public readonly WeaponPixelData[,] WeaponDataLeft;
        public readonly WeaponPixelData[,] WeaponDataRight;
        public readonly WeaponPixelData[,] WeaponDataStab;
        public readonly WeaponForwardDirection ForwardCorner;


        public readonly OneSideAnalyserHelper LeftAnalyser;
        public readonly OneSideAnalyserHelper RightAnalyser;
        public readonly OneSideAnalyserHelper StabAnalyser;

        public readonly List<BrokenPixel> BrokenPixels = new List<BrokenPixel>();
        
        // public float _paramP = 0.57f;
        // public float _paramK = 1.02f;
        // public float _paramY = 1.9f;
        // public float _paramE = 2.52f;

        public float[] _attenuationLevels = new [] {-1, 0.01f, 0.9f, 3.6f, 7.52f, 18.62f};

        private Vector2Int _gridOrigin;
        public Vector2 Origin { get; private set; }
        public float Length { get; private set; }
        public float Mass { get; private set; }
        public float Inertia { get; private set; }
        
        public float TotalDamageLeft { get; private set; }
        public float TotalDamageRight { get; private set; }
        public float TotalDamageStab { get; private set; }

        public PixelWeaponAnalyser(PixelWeapon weapon)
        {
            Weapon = weapon;
            WeaponDataLeft = new WeaponPixelData[weapon.Size.x, weapon.Size.y];
            WeaponDataRight = new WeaponPixelData[weapon.Size.x, weapon.Size.y];
            WeaponDataStab = new WeaponPixelData[weapon.Size.x, weapon.Size.y];

            switch (weapon.ForwardDirection)
            {
                case WeaponForwardDirection.TopLeft:
                    _gridOrigin = new Vector2Int(weapon.Size.x, 0);
                    Origin = new Vector2(.5f, -.5f);
                    
                    LeftAnalyser = new OneSideAnalyserHelper()
                    {
                        Tangent = new Vector2(-1, 1).normalized,
                        Normal = new Vector2(-1, -1).normalized,
                        OcclusionClearValue = int.MaxValue,
                        OccludedPixels = new Utility.OffsetArray<int>(-weapon.Size.y - 3, weapon.Size.x + weapon.Size.y + 4),
                        PixelIterator = Utility.DiagonalIndices(weapon.Size.x, weapon.Size.y, Utility.RectCorner.XMinYMin),
                        OcclusionCoordMap = (x, y) => (x, y),
                        OcclusionCompareMode = PixelCompareMode.Less,
                        DamageAttenuationField = new float[weapon.Size.x, weapon.Size.y],
                        IgnorePixel = (x, y, pixelX, pixelY) => (x + y) < (pixelX + pixelY),
                        AttenuationFieldParams = new AttenuationFieldParameters()
                        {
                            p = 0.57f,
                            k = 1.02f,
                            y = 1.9f,
                            e = 2.52f,
                        },
                    };

                    RightAnalyser = new OneSideAnalyserHelper()
                    {
                        Tangent = new Vector2(-1, 1).normalized,
                        Normal = new Vector2(1, 1).normalized,
                        OcclusionClearValue = int.MinValue,
                        OccludedPixels = new Utility.OffsetArray<int>(-weapon.Size.y - 3, weapon.Size.x + weapon.Size.y + 4),
                        PixelIterator = Utility.DiagonalIndices(weapon.Size.x, weapon.Size.y, Utility.RectCorner.XMaxYMax),
                        OcclusionCoordMap = (x, y) => (x, y),
                        OcclusionCompareMode = PixelCompareMode.Greater,
                        DamageAttenuationField = new float[weapon.Size.x, weapon.Size.y],
                        IgnorePixel = (x, y, pixelX, pixelY) => (x + y) > (pixelX + pixelY),
                        AttenuationFieldParams = new AttenuationFieldParameters()
                        {
                            p = 0.57f,
                            k = 1.02f,
                            y = 1.9f,
                            e = 2.52f,
                        },
                    };

                    StabAnalyser = new OneSideAnalyserHelper()
                    {
                        Tangent = new Vector2(1, 1).normalized,
                        Normal = new Vector2(-1, 1).normalized,
                        OcclusionClearValue = int.MaxValue,
                        OccludedPixels = new Utility.OffsetArray<int>(-2, weapon.Size.x + weapon.Size.y + 4),
                        PixelIterator = Utility.DiagonalIndices(weapon.Size.x, weapon.Size.y, Utility.RectCorner.XMinYMax),
                        OcclusionCoordMap = (x, y) => (y, x),
                        OcclusionCompareMode = PixelCompareMode.Less,
                        DamageAttenuationField = new float[weapon.Size.x, weapon.Size.y],
                        IgnorePixel = (x, y, pixelX, pixelY) => (x - y) < (pixelX - pixelY),
                        AttenuationFieldParams = new AttenuationFieldParameters()
                        {
                            p = 0.09f,
                            k = 0.18f,
                            y = 11.3f,
                            e = 6.34f,
                        },
                    };
                    
                    
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void UpdateWeaponData()
        {
            
            for(var y = 0;y<Weapon.Size.y; y++)
            for (var x = 0; x < Weapon.Size.x; x++)
            {
                WeaponDataLeft[x, y] = new WeaponPixelData()
                {
                    Pixel = Weapon[x, y],
                    DamageRate = 1,
                    WearRate = 1,
                };
                WeaponDataRight[x, y] = new WeaponPixelData()
                {
                    Pixel = Weapon[x, y],
                    DamageRate = 1,
                    WearRate = 1,
                };
                WeaponDataStab[x, y] = new WeaponPixelData()
                {
                    Pixel = Weapon[x, y],
                    DamageRate = 1,
                    WearRate = 1,
                };
            }
            
            LeftAnalyser.Reset();
            RightAnalyser.Reset();
            StabAnalyser.Reset();
            
            AnalyseStructure();
            
            AnalyseOneSide(LeftAnalyser);
            AnalyseOneSide(RightAnalyser);
            AnalyseOneSide(StabAnalyser);

            GenerateWeaponData();
            
            PhysicalAnalyse();
        }

        private void AnalyseStructure()
        {
            Pixel GetPixelAt(int x, int y)
            {
                if (x < 0 || x >= Weapon.Size.x || y < 0 || y >= Weapon.Size.y)
                    return null;
                return Weapon.Pixels[x, y];
            }
            Dictionary<Pixel, Pixel> unionRoot = new Dictionary<Pixel, Pixel>();

            Pixel GetRootOf(Pixel pixel)
            {
                if (unionRoot.TryGetValue(pixel, out var root))
                {
                    if (root == pixel)
                        return pixel;

                    var rootOfRoot = GetRootOf(root);
                    unionRoot[root] = rootOfRoot;
                    return rootOfRoot;
                }
                else
                {
                    unionRoot[pixel] = pixel;
                    return pixel;
                }
            }
            
            BrokenPixels.Clear();
            for (var y = 0; y < Weapon.Size.y; y++)
            for (var x = 0; x < Weapon.Size.x; x++)
            {
                if(!Weapon.Pixels[x,y])
                    continue;
                if (Weapon.Pixels[x, y].Endurance <= 0)
                {
                    var brokenPixel = new BrokenPixel()
                    {
                        Pixel = Weapon.Pixels[x, y],
                        Position = new Vector2Int(x, y),
                    };
                    BrokenPixels.Add(brokenPixel);
                    Weapon.Pixels[x, y] = null;
                }
            }
            Debug.Log($"{BrokenPixels.Count} pixels broken due to low endurance");
            var enduranceBroken = BrokenPixels.Count;
            // return;


            Utility.RectCorner startCorner;
            Pixel handelPixel;
            switch (Weapon.ForwardDirection)
            {
                case WeaponForwardDirection.TopLeft:
                    startCorner = Utility.RectCorner.XMaxYMin;
                    handelPixel = Weapon.Pixels[Weapon.HandelPosition.x, Weapon.HandelPosition.y];
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var (x, y) in Utility.DiagonalIndices(Weapon.Size.x, Weapon.Size.y, startCorner))
            {
                if (!Weapon.Pixels[x, y])
                    continue;
                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var neighbor = GetPixelAt(x + dx, y + dy);
                    if (!neighbor || !neighbor.Type)
                        continue;

                    unionRoot[GetRootOf(neighbor)] = GetRootOf(Weapon.Pixels[x, y]);
                }
            }


            for (var y = 0; y < Weapon.Size.y; y++)
            for (var x = 0; x < Weapon.Size.x; x++)
            {
                if(!Weapon.Pixels[x,y])
                    continue;
                if (GetRootOf(Weapon.Pixels[x, y]) != GetRootOf(handelPixel))
                {
                    var brokenPixel = new BrokenPixel()
                    {
                        Pixel = Weapon.Pixels[x, y],
                        Position = new Vector2Int(x, y),
                    };
                    BrokenPixels.Add(brokenPixel);
                    Weapon.Pixels[x, y] = null;
                }
            }
            Debug.Log($"{BrokenPixels.Count - enduranceBroken} pixels broken due to structure broken");
        }

        void PhysicalAnalyse()
        {

            Mass = 0;
            Inertia = 0;
            Length = 0;

            for (var y = 0; y < Weapon.Size.y; y++)
            for (var x = 0; x < Weapon.Size.x; x++)
            {
                if(Weapon[x, y] is null)
                    continue;

                var r = Vector2.Distance(new Vector2(x, y), _gridOrigin);
                Length = Mathf.Max(Length, r);
                Inertia += Weapon[x, y].Weight * r * r;
                Mass += Weapon[x, y].Weight;
            }
        }

        void AnalyseOneSide(OneSideAnalyserHelper helper)
        {
            foreach (var (posX, posY) in helper.PixelIterator)
            {
                var diagonalPos = ToDiagonal(new Vector2Int(posX, posY));

                var (occludeX, occludeY) = helper.OcclusionCoordMap(diagonalPos.x, diagonalPos.y);
                var occluded = PixelOccluded(occludeX, occludeY, helper.OccludedPixels, helper.OcclusionCompareMode);
                if (occluded)
                {
                    helper.DamageAttenuationField[posX, posY] = float.MaxValue;
                    continue;
                    
                }

                if (!Weapon[posX, posY])
                    continue;

                helper.OccludedPixels[occludeX] = occludeY;

                for (var y = 0; y < Weapon.Size.y; y++) 
                for (var x = 0; x < Weapon.Size.x; x++)
                {
                    if(x == posX && y == posY)
                        continue;
                    if(helper.IgnorePixel(x, y, posX, posY))
                        continue;
                
                    var delta = new Vector2(x, y) - new Vector2(posX, posY);
                    
                    var attenuation = PixelAttenuation(delta, helper.Tangent, -helper.Normal, helper.AttenuationFieldParams);
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

        float PixelAttenuation(Vector2 delta, Vector2 tangentVector, Vector2 normalVector, AttenuationFieldParameters param)
        {
            delta = new Vector2(Vector2.Dot(delta, tangentVector), Vector2.Dot(delta, normalVector));
            delta.y += param.y;
            
            var p = param.p;
            var z = 2 * p * delta.y - delta.x * delta.x;
            if (z < 0)
                return 0;
            else
            {
                z = Mathf.Sqrt(z);
                z = Mathf.Pow(z, param.e) * param.k;
                return z;
            }
        }

        void GenerateWeaponData()
        {
            TotalDamageLeft = 0;
            TotalDamageRight = 0;
            TotalDamageStab = 0;
            for (var y = 0; y < Weapon.Size.y; y++)
            for (var x = 0; x < Weapon.Size.x; x++)
            {
                if (Weapon[x, y] is null)
                {
                    WeaponDataLeft[x, y] = WeaponDataRight[x, y] = WeaponDataStab[x, y] = new WeaponPixelData()
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
                WeaponDataStab[x, y] = GenerateWeaponData(x, y, r, StabAnalyser.DamageAttenuationField[x, y]);

                TotalDamageLeft += WeaponDataLeft[x, y].Damage;
                TotalDamageRight += WeaponDataRight[x, y].Damage;
                TotalDamageStab += WeaponDataStab[x, y].Damage;
            }
        }

        WeaponPixelData GenerateWeaponData(int x, int y, float distance, float damageAttenuation)
        {
            // will never get -1 here, since it is only for null pixels.
            var attenuationLevel = MathUtility.ListRangeMap(
                _attenuationLevels,
                new[] {-1, 0, 1, 2, 3, 4}, damageAttenuation);

            var attenuation = 1f;
            switch (attenuationLevel)
            {
                case -1: 
                case 4:
                    attenuation = 0;
                    break;
                case 1:
                    attenuation = 0.8f;
                    break;
                case 2:
                    attenuation = 0.5f;
                    break;
                case 3:
                    attenuation = 0.3f;
                    break;
            }

            var pixel = Weapon[x, y];
            if (!pixel)
            {
                attenuation = 0;
            }
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