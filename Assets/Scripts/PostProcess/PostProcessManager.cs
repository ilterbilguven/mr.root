using System;
using System.Collections;
using DG.Tweening;
using MrRoot.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MrRoot
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessManager : SingletonBehaviour<PostProcessManager>
    {
        public static event Action<bool> BlackAndWhite;
        
        [SerializeField] private float _blackAndWhiteDuration = 2f;
        [SerializeField] private float _fadeDuration = 0.2f;

        private bool _blackAndWhiteActive;

        private Volume _volume;
        private ColorAdjustments _colorAdjustments;
        private Tweener _colorAdjustTween;
        private void Awake()
        {
            _volume = GetComponent<Volume>();
            if (!_volume.profile.TryGet(out _colorAdjustments))
            {
                Debug.LogError("No color adjustment is added to Volume component");
                return;
            }

            RaySlicer.FlagHit += OnFlagHit;
        }

        protected override void OnDestroy()
        {
            RaySlicer.FlagHit -= OnFlagHit;
            base.OnDestroy();
        }

        private void OnFlagHit()
        {
            if (_blackAndWhiteActive)
                return;
            
            StartCoroutine(FlagHitRoutine());
        }

        private void ToggleBlackAndWhite(bool active)
        {
            if (!_colorAdjustments)
                return;
            
            _colorAdjustTween.Kill();
            _colorAdjustTween = DOTween.To(() => _colorAdjustments.saturation.value,
                x => _colorAdjustments.saturation.value = x,
                active ? -100 : 0, _fadeDuration);
            
            BlackAndWhite?.Invoke(active);
        }

        private IEnumerator FlagHitRoutine()
        {
            _blackAndWhiteActive = true;
            ToggleBlackAndWhite(true);
            yield return new WaitForSeconds(_blackAndWhiteDuration);
            ToggleBlackAndWhite(false);
            _blackAndWhiteActive = false;
        }
    }
}

