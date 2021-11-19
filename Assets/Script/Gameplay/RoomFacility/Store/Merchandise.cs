using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchandise : MonoBehaviour
{
    private Merchant _merchant;
    [SerializeField] private String _description;

    void Start()
    {
        _merchant = GameObject.Find("Merchant").GetComponent<Merchant>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_merchant.IsDisplaying)
            _merchant.ShowDescription(_description);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && _merchant.IsDisplaying)
            _merchant.HideDescription();
    }
}
