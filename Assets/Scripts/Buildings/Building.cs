using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MrRoot.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MrRoot
{
    public class Building : MonoBehaviour
    {
        public event Action Captured;
        private bool _captured;

        private void Awake()
        {
            GameManager.Instance.RegisterBuilding(this);
        }

        [Button]
        public void Capture()
        {
            if (_captured)
            {
                return;
            }
            
            if (TryGetComponent(out Renderer buildingRenderer))
            {
                foreach (var material in buildingRenderer.materials)
                {
                    material.DOColor(Color.black, 0.5f);
                }

            }
            
            _captured = true;
            Captured?.Invoke();
        }
    }
}
