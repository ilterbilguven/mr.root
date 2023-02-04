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

        private void Awake()
        {
            GameManager.Instance.RegisterBuilding(this);
        }

        [Button]
        public void Capture()
        {
            if (TryGetComponent(out Renderer renderer))
            {
                renderer.material.DOColor(Color.black, 0.5f);
            }
            
            Captured?.Invoke();
        }
    }
}
