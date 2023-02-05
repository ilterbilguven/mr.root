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

        [MinMaxSlider(2f, 10f)] [SerializeField] private Vector2 _durationRange = new Vector2(2f, 10f);
        
        
        [Button]
        public void Initialize()
        {
                
        }

        [Button]
        public void SpawnRoot()
        {
            var root = Instantiate(_rootPrefab);
            root.Initialize(GetRandomAvailableBuilding(), Random.Range(_durationRange.x, _durationRange.y));
        }

        private Transform GetRandomAvailableBuilding()
        {
            var buildings = GameManager.Instance.Buildings;
            
            return buildings[Random.Range(0, buildings.Count)].transform;
        }
        
    }
}
