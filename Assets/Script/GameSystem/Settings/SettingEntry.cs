using System;

namespace Prototype.Settings
{
    
    [Serializable]
    public abstract class SettingEntryBase
    {
        internal abstract void RegisterSetting(SettingEntryBase setting);
    }

    public class SettingEntry<T> : SettingEntryBase where  T : SettingEntry<T>
    {
        private static T _currentSettings;
        public static T Current
        {
            get
            {
                if (_currentSettings is null)
                {
                    _currentSettings = GameSettings.GetSetting<T>();
                }

                return _currentSettings;
            }
            set => _currentSettings = value;
        }
        
        internal override void RegisterSetting(SettingEntryBase setting)
        {
            Current = setting as T;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class GameSettingAttribute : Attribute
    {
        public GameSettingAttribute()
        {
        }
    }
}