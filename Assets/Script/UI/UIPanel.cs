﻿using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GlobalUIPanel<T> : Singleton<T> where T : GlobalUIPanel<T>
    {
        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(float time = .2f)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Utility.ShowUI(canvasGroup, time));
        }

        public void Hide(float time = .2f)
        {
            StopAllCoroutines();
            StartCoroutine(Utility.HideUI(canvasGroup, time, true));
        }
    }
}