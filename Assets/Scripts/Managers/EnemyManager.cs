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
        [MinMaxSlider(0.5f, 3f)] [SerializeField] private Vector2 _spawnCooldown = new Vector2(0.5f, 3f);
        
        
        [Button]
        public void Initialize()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (!GameManager.Instance.IsGameOver)
            {
                yield return new WaitForSeconds(Random.Range(_spawnCooldown.x, _spawnCooldown.y));
                
                SpawnRoot();
            }
        }

        [Button]
        public void SpawnRoot()
        {
            var root = Instantiate(_rootPrefab);

            var building = GetRandomAvailableBuilding();

            if (!building)
            {   
                return;
            }
            
            root.Initialize(building, Random.Range(_durationRange.x, _durationRange.y));
        }

        private Transform GetRandomAvailableBuilding()
        {
            var buildings = GameManager.Instance.Buildings;

            if (buildings.Count == 0)
            {
                return null;
            }
            
            return buildings[Random.Range(0, buildings.Count)].transform;
        }
        
    }
}
