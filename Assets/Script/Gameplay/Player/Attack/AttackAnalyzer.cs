using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Element;
using Prototype.Rendering;
using Prototype.Settings;
using Prototype.Utils;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(PixelImageRenderer))]
    public class AttackAnalyzer : MonoBehaviour
    {
        private PixelWeapon _currentWeapon;
        [SerializeField] private PixelWeaponAnalyser _analyser;

        private PixelImageRenderer _pixelImageRenderer;

        public PixelWeapon CurrentWeapon
        {
            get => _currentWeapon;
            set
            {
                _currentWeapon = value;
                _analyser = new PixelWeaponAnalyser(value);
                UpdateAnalyser();
            }
        }

        private void Awake()
        {
            _pixelImageRenderer = GetComponent<PixelImageRenderer>();
        }
        
        public void UpdateAnalyser() 
	    {
            _analyser.UpdateWeaponData();
            CurrentWeapon.UpdateTexture();
            foreach (var brokenPixel in _analyser.BrokenPixels)
            {
                var worldPos = _pixelImageRenderer.ImageToWorldPos(brokenPixel.Position);
                GlobalEffectManager.Instance.PixelBrokenEffect.Emit(new ParticleSystem.EmitParams()
                {
                    applyShapeToPosition = true,
                    position = worldPos,
                }, 30);
            }
	    }

        public float ResolveDamageValue(AttackType type)
        {
            WeaponPixelData[,] data;
            float damage = 0;
            switch (type)
            {
                case AttackType.L:
                    data = _analyser.WeaponDataLeft;
                    break;
                case AttackType.M:
                    data = _analyser.WeaponDataStab;
                    break;
                case AttackType.R:
                    data = _analyser.WeaponDataRight;
                    break;
                case AttackType.Rotate:
                    data = _analyser.WeaponDataStab;
                    break;
                default:
                    data = null;
                    break;
            }

            foreach(WeaponPixelData d in data)
            {
                damage += d.Damage;

                if (d.Pixel)
                {
                    d.Pixel.Endurance -= d.WearRate;
                }
            }
            // handel pixel endurance lost and structure broken
            UpdateAnalyser();
            

            return damage;
        }

    }

}

