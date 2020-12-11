using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Prototype.Settings
{
    public class GameSettings : ScriptableObject
    {
        #region Internal Codes

        private const string FileName = "GameSettings.asset";
        private const string SaveFolder = "Settings";
        private const string SavePath = "Assets/" + SaveFolder + "/" + FileName;

        internal static GameSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<GameSettings>(SavePath);
            if (!settings)
            {
                settings = CreateInstance<GameSettings>();
                if (!AssetDatabase.IsValidFolder(SaveFolder))
                {
                    AssetDatabase.CreateFolder("Assets", SaveFolder);
                }
                
                AssetDatabase.CreateAsset(settings, SavePath);
                AssetDatabase.SaveAssets();
                var preloadAssets = PlayerSettings.GetPreloadedAssets().ToList();
                preloadAssets.Add(settings);
                PlayerSettings.SetPreloadedAssets(preloadAssets.ToArray());
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        internal static IEnumerable<FieldInfo> GetSettingFields()
        {
            return typeof(GameSettings).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(field => field.FieldType.IsSubclassOf(typeof(SettingEntryBase)))
                .Where(field => field.GetCustomAttributes(true).Any(attr => attr is GameSettingAttribute));
        }

        internal static T GetSetting<T>() where T : SettingEntry<T>
        {
            return GetSettingFields()
                .Where(field => field.FieldType == typeof(T))
                .FirstOrDefault()?.GetValue(GetOrCreateSettings()) as T;
        }

        private void Awake()
        {
            typeof(GameSettings).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(field=>field.FieldType.IsSubclassOf(typeof(SettingEntryBase)))
                .Where(field => field.GetCustomAttributes(true).Any(attr => attr is GameSettingAttribute))
                .ForEach(field =>
                {
                    var setting = field.GetValue(this) as SettingEntryBase;
                    setting.RegisterSetting(setting);
                });
        }

        #endregion

        [GameSetting] 
        [SerializeField]
        private PixelAssets PixelAssets = new PixelAssets();

        [GameSetting] 
        [SerializeField] 
        private UISettings UISettings = new UISettings();

        [GameSetting]
        [SerializeField]
        private GamePrefabs GamePrefabs = new GamePrefabs();
    }

    static class GameSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateGameSettingsProvider()
        {
            var provider = new SettingsProvider("Project/PixelPrototype", SettingsScope.Project)
            {
                label = "Pixel Prototype Game Settings",
                guiHandler = (searchContext) =>
                {
                    var settings = GameSettings.GetSerializedSettings();
                    foreach (var info in GameSettings.GetSettingFields())
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty(info.Name));
                    }

                    settings.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(GameSettings.GetOrCreateSettings());
                },
            };
            return provider;
            
        }
        
    }
}