﻿using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {
        private CanvasGroup canvas;
        [SerializeField] private TextMeshProUGUI infoTxt;

        public void SetText(string message) => infoTxt.SetText(message);

        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
        }

        public void Show(float fadeDuration = 0.5f)
        {
            canvas.DOFade(endValue: 1.0f, duration: fadeDuration).SetEase(Ease.OutSine);
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }

        public void Hide(float fadeDuration = 0.5f)
        {
            canvas.DOFade(endValue: 0.0f, duration: fadeDuration).SetEase(Ease.InSine);
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }

        public void StartGame() => OnStartClicked.Invoke();
        public static UnityEvent OnStartClicked = new UnityEvent();
    }
}
