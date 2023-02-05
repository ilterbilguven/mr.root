using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MrRoot.Managers
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        private Image _timer;
        private TimeSpan _remainingTime;

        public event Action TimesUp;
        
        private void Awake()
        {
            TryGetComponent(out _timer);
            
            GameManager.Instance.GameOver += OnGameOver;
        }
        private void OnGameOver(bool obj)
        {
            _timer.DOKill();
        }

        [Button]
        public void Initialize(int sessionTime)
        {
            _timer.DOFillAmount(0, sessionTime);
        }
    }
}
