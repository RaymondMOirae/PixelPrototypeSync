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
    public class AttackAnalyzer : MonoBehaviour
    {
        private PixelWeapon _currentWeapon;
        [SerializeField] private float DamageScale = 1;
        private PixelWeaponAnalyser _analyser;

        [SerializeField] private PixelImageRenderer _pixelImageRenderer;
        [SerializeField] private Transform _weaponCollider;

        public PixelWeapon CurrentWeapon
        {
            get => _currentWeapon;
            set
            {
                _currentWeapon = value;
                _analyser = new PixelWeaponAnalyser(value);
                UpdateAnalyser();
                _pixelImageRenderer.SetPixelImage(value);
            }
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

            _weaponCollider.localScale = new Vector3(_analyser.Length * _pixelImageRenderer.PixelRenderSize, 1, 1);
        }

        public float ResolveDamageValue(AttackType type)
        {
            WeaponData data = default;
            float damage = 0;
            switch (type)
            {
                case AttackType.L:
                    data = _analyser.LeftSideData;
                    break;
                case AttackType.M:
                    data = _analyser.SpikeData;
                    break;
                case AttackType.R:
                    data = _analyser.RightSideData;
                    break;
                case AttackType.Rotate:
                    data = _analyser.SpikeData;
                    break;
                default:
                    break;
            }

            foreach(var pixelData in data.AvailablePixels)
            {
                // damage += d.Damage;

                if (pixelData.Pixel)
                {
                    pixelData.Pixel.Endurance -= pixelData.WearRate;
                }
            }

            damage = data.TotalPhysicalDamage;
            damage *= DamageScale;
            
            // handel pixel endurance lost and structure broken
            UpdateAnalyser();
            

            return damage;
        }

    }

}

