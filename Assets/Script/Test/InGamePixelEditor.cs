using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Element;
using Prototype.Inventory;
using Prototype.Rendering;
using Prototype.Settings;
using Prototype.UI;
using Prototype.Utils;
using Script.GameSystem;
using UnityEngine;
using Prototype.Gameplay.Player;
using Prototype.Gameplay.Player.Attack;
using Prototype.Script.Test;

namespace Prototype.Inventory
{
    public class InGamePixelEditor : MonoBehaviour
    {
        public int Count = 32;
        private List<Pixel> pixels = new List<Pixel>();
        public Inventory Inventory;
        private PlayerController _player;
        private WeaponController _weapon;
        
        private void Start(){
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
            _weapon = _player.Weapon;
            //_renderer = _weapon.WeaponRenderer;

            Inventory = _player.GetComponent<PlayerPackage>().Inventory;
            // foreach (var pixelType in PixelAssets.Current.PixelTypes.Where(p=>p.Attribute == PixelAttribute.Default))
            // {
            //     for (var i = 0; i < Count; i++)
            //     {
            //         Inventory.SaveItem(new Pixel(pixelType));
            //     }
            // }
            
            //PixelImage image = _weapon.WeaponImage;
            
            // Inventory.SaveItem(_weapon.CurrentWeapon);
            
            //await PixelEditor.Instance.Edit(Inventory);

            //_weapon.UpdateWeapon();
            //_renderer.SetPixelImage(image);
        }

        public async void ShowEditor()
        {
            await PixelEditor.Instance.Edit(Inventory);
            //_weapon.WeaponRenderer.SetPixelImage(_weapon.WeaponRenderer.Image);
            _weapon.UpdateWeapon();
        }
    }
}