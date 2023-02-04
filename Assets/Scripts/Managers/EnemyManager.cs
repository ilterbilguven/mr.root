using System.Collections;
using System.Collections.Generic;
using MrRoot.Root;
using ProceduralModeling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MrRoot.Managers
{
    public class EnemyManager : SingletonBehaviour<EnemyManager>
    {

        [SerializeField] private Transform _rootPoint;
        [SerializeField] private EnemyRoot _rootPrefab;
        
        [Button]
        public void Initialize()
        {
                
        }

        [Button]
        private void SpawnRoot()
        {
            var root = Instantiate(_rootPrefab);
            root.Initialize(GetRandomAvailableBuilding());
        }

        private Transform GetRandomAvailableBuilding()
        {
            var buildings = GameManager.Instance.Buildings;
            
            return buildings[Random.Range(0, buildings.Count)].transform;
        }
        
    }
}
