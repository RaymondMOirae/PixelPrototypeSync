using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void SetWeaponDisplay(Sprite spr)
    {
        _img.sprite = spr;
    }


}
