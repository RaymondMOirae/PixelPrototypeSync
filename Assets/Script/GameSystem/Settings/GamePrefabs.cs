using System;
using UnityEngine;

namespace Prototype.Settings
{
    [Serializable]
    public class GamePrefabs : SettingEntry<GamePrefabs>
    {
        public GameObject DroppedItem;
    }
}