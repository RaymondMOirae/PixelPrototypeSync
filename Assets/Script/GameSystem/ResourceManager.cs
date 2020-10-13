using System.Collections.Generic;
using Prototype.Element;
using Prototype.Utils;
using UnityEngine;

namespace Script.GameSystem
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [Header("Prefabs")]
        
        public GameObject PrefabPixelSlot;

        public GameObject PrefabPixelItem;

        [Header("Resources")] 
        public List<PixelElement> PixelElements = new List<PixelElement>();
    }
}