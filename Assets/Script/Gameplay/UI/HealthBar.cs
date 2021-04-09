using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Gameplay.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        public float MaxHealth
        {
            get { return slider.maxValue; }
            set { slider.maxValue = value; }
        }

        public float CurrentHealth
        {
            get { return slider.value; }
            set { slider.value = value; }
        }

        public void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void RestoreMaxHealth()
        {
            CurrentHealth = MaxHealth;
        }
    }
}

