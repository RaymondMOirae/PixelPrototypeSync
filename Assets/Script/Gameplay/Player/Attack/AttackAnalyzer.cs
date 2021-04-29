using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Element;
using Prototype.Rendering;

namespace Prototype.Gameplay.Player.Attack
{
    public class AttackAnalyzer : MonoBehaviour
    {
        private PixelWeapon _currentWeapon;
        [SerializeField] private PixelWeaponAnalyser _analyser;

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

        // Start is called before the first frame update
        private void Start()
        {
            // _renderer = GetComponent<PixelImageRenderer>();
            // var weapon = PixelWeapon.CreateFromPixelImage(_renderer.Image, new Vector2Int(7, 0),
            //     WeaponForwardDirection.TopLeft);
            // _analyser = new PixelWeaponAnalyser(weapon);
            // _analyser.UpdateWeaponData();
        }

        public void UpdateAnalyser() 
	    {
            _analyser.UpdateWeaponData();
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
            CurrentWeapon.UpdateTexture();
            

            return damage;
        }

    }

}

