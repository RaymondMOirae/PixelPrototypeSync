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

namespace Prototype.Script.Test
{
    public class TestPixelEditor : MonoBehaviour
    {
        public int Count = 32;
        private List<Pixel> pixels = new List<Pixel>();
        public Inventory.Inventory Inventory;
        private PixelImageRenderer renderer;
        private WeaponController _weapon;
        
        private async void Start(){
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            _weapon = player.Weapon;
            renderer = _weapon.WeaponRenderer;

            Inventory = player.GetComponent<PlayerPackage>().Inventory;
            foreach (var pixelType in PixelAssets.Current.PixelTypes.Where(p=>p.Attribute == PixelAttribute.Default))
            {
                for (var i = 0; i < Count; i++)
                {
                    Inventory.SaveItem(new Pixel(pixelType));
                }
            }
            
            PixelImage image = _weapon.WeaponImage;

            
            Inventory.SaveItem(image);
            
            await PixelEditor.Instance.Edit(Inventory);

            image.UpdateTexture();
            renderer.SetPixelImage(image);

        }

        private async void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
            {
                await PixelEditor.Instance.Edit(Inventory);
            }
        }
    }
}