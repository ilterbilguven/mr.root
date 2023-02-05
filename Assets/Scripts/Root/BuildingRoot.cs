using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MrRoot.Managers;
using MrRoot.Root;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MrRoot
{
    public class BuildingRoot : MonoBehaviour
    {
        public GameObject _rootPrefab;

        private IEnumerator Start()
        {
            EnemyRoot.RootCut += OnRootCut;
            
            yield return new WaitForSeconds(0.1f);
            
            Grow();
        }

        private void OnDestroy()
        {   
            EnemyRoot.RootCut -= OnRootCut;
        }
        
        [Button]
        private void OnRootCut()
        {
            if (TryGetComponent(out Renderer buildingRenderer))
            {
                foreach (var material in buildingRenderer.materials)
                {
                    material.DOKill(true);
                    
                    material.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo);
                }
            }
        }

        public void Grow()
        {
            var root = Instantiate(_rootPrefab, transform);

            if (root.TryGetComponent(out Renderer rootRenderer))
            {
                rootRenderer.material.DOFloat(1f, "_Transition", 2f).From(0f);
            }
            
            root.transform.DORotate(Vector3.up * Random.Range(-360f, 360f), 2f);

            //root.transform.DOScale(1, 2f).From(0f);
        }
    }
}
