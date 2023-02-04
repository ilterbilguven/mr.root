using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MrRoot.Managers
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        private Stopwatch _stopwatch = new Stopwatch();
        [SerializeField] [ReadOnly] private int _sessionTime;
        private TMP_Text _timerText;
        private TimeSpan _remainingTime;

        public event Action TimesUp;
        
        private void Awake()
        {
            TryGetComponent(out _timerText);
        }

        [Button]
        public void Initialize(int sessionTime)
        {
            _sessionTime = sessionTime;
            _remainingTime = TimeSpan.FromSeconds(_sessionTime);
            SetRemainingTimeText(_remainingTime);

            StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            _stopwatch.Start();
            
            do
            {
                var remainingTime = _remainingTime - _stopwatch.Elapsed;
                SetRemainingTimeText(remainingTime);
                yield return new WaitForFixedUpdate();
            } while (_stopwatch.Elapsed.TotalSeconds <= _sessionTime && !GameManager.Instance.IsGameOver);
            
            TimesUp?.Invoke();
            
            _stopwatch.Stop();
        }
        
        private void SetRemainingTimeText(TimeSpan value)
        {
            _timerText.text = $"<mspace=50>{value.Minutes:00}:{value.Seconds:00}</mspace>";
        }
    }
}
