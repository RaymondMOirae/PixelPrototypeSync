using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Gameplay.UI
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        public void Awake()
        {
            slider = GetComponent<Slider>();
        }
        public void SetMaxHealth(float maxHealth)
        {
            slider.maxValue = maxHealth;
        }

        public void RecoverMaxHealth()
        {
            SetHealth((int)slider.maxValue);
        }

        public void SetHealth(float health)
        {
            slider.value = health;
        }

    }

}

