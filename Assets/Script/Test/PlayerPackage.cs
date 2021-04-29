using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Element;
using Prototype.Gameplay.Player;
using Prototype.Inventory;
using Prototype.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.Script.Test
{
    public class PlayerPackage : MonoBehaviour
    {
        public Inventory.Inventory Inventory = new Inventory.Inventory();

        public ItemPicker ItemPicker;
        
        public List<PixelImageAsset> TestPixelImages = new List<PixelImageAsset>();

        private void Awake()
        {
            ItemPicker.OnItemPicked.AddListener(OnItemPicked);
            
            foreach (var pixelType in PixelAssets.Current.PixelTypes.Where(p=>p.Attribute == PixelAttribute.Default))
            {
                for (var i = 0; i < 16; i++)
                {
                    Inventory.SaveItem(new Pixel(pixelType));
                }
            }

            foreach (var image in TestPixelImages)
            {
                var weapon = PixelWeapon.CreateFromPixelImage(image.Image.Clone(true),
                    new Vector2Int(image.Width - 1, 0), WeaponForwardDirection.TopLeft);
                weapon.UpdateTexture();
                
                Inventory.SaveItem(weapon);
            }
        }
        
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                var mousePos = UnityEngine.Input.mousePosition;
                var pos = Camera.main.ScreenToWorldPoint(mousePos);

                Inventory.DropOne(Inventory.ItemGroups.RandomTake(Random.value).ItemType, pos);
            }
        }

        void OnItemPicked(DroppedItem item)
        {
            Inventory.PickUp(item);
        }

    }
}