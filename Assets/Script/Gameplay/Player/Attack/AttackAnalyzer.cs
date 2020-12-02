using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Element;
using Prototype.Rendering;

namespace Prototype.Gameplay.Player.Attack
{
    public class AttackAnalyzer : MonoBehaviour
    {
        [SerializeField] private PixelImageAsset _asset;
        [SerializeField] private PixelWeaponAnalyser _analyser;
        private PixelImageRenderer _renderer;

        // Start is called before the first frame update
        private void Awake()
        {
            _renderer = GetComponent<PixelImageRenderer>();
            _analyser = new PixelWeaponAnalyser(_renderer.Image, WeaponForwardDirection.TopLeft);
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

