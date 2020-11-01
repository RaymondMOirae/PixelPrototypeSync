using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Element;

namespace Prototype.Gameplay.Player
{
    public class Weapon : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        [SerializeField] private PixelImageAsset _asset;
        [SerializeField] private PixelWeaponAnalyser _analyser;
        // Start is called before the first frame update
        private void Awake()
        {

            _analyser = new PixelWeaponAnalyser(_asset.Image, WeaponForwardDirection.TopLeft);
            _analyser.UpdateWeaponData();
            _renderer = GetComponent<SpriteRenderer>();
            
            //_renderer.
        }

        public float ResolveDamage(Attack type)
        {
            WeaponPixelData[,] data;
            float damage = 0;
            switch (type)
            {
                case Attack.L:
                    data = _analyser.WeaponDataLeft;
                    break;
                case Attack.M:
                    data = _analyser.WeaponDataStab;
                    break;
                case Attack.R:
                    data = _analyser.WeaponDataStab;
                    break;
                default:
                    data = null;
                    break;
            }

            foreach(WeaponPixelData d in data)
            {
                damage += d.Damage;
            }

            return damage;
        }

    }

}

