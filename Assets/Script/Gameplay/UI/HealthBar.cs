using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
    }
    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    public void RecoverMaxHealth()
    {
        SetHealth((int)slider.maxValue);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

}
