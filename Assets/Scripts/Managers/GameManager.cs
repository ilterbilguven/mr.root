using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MrRoot.Root;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace MrRoot.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public event Action GameStarted; 
        public event Action<bool> GameOver;
        public int SessionTime = 60;
        public List<Building> Buildings = new List<Building>();
        [field: SerializeField] [field: ReadOnly] public bool IsGameOver { get; private set; }
        [ReadOnly] [SerializeField] private int _buildingCount;

        private void Awake()
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        }

        private IEnumerator Start()
        {
            
            yield return new WaitForSeconds(3f);
            
            Initialize();
            
            
            GameStarted?.Invoke();
        }


        [Button]
        public void Initialize()
        {
            _buildingCount = Buildings.Count;
            
            TimeManager.Instance.TimesUp += OnTimesUp;
            TimeManager.Instance.Initialize(SessionTime);
            
            EnemyManager.Instance.Initialize();
        }

        private void OnTimesUp()
        {
            TimeManager.Instance.TimesUp -= OnTimesUp;

            if (Buildings.Count > _buildingCount / 2)
            {
                // todo: bonus maybe?
                Debug.Log("****** *** ****'** ****** ******!");
                Win();
            }
            else
            {
                Debug.Log("We lose.");
                Lose();
            }
        }

        public void RegisterBuilding(Building building)
        {
            if (Buildings.Contains(building))
            {
                return;
            }
            
            Buildings.Add(building);
            building.Captured += BuildingOnCaptured;

            void BuildingOnCaptured()
            {
                building.Captured -= BuildingOnCaptured;

                Buildings.Remove(building);

                if (Buildings.Count < _buildingCount / 2)
                {
                    Lose();
                }
            }
        }

        private void Win()
        {
            if (IsGameOver)
            {
                Debug.LogError("Stop, stop! He's already dead!");
                return;
            }
            
            IsGameOver = true;
            GameOver?.Invoke(true);
            SceneManager.LoadScene(2);
        }

        private void Lose()
        {
            if (IsGameOver)
            {
                Debug.LogError("Stop, stop! He's already dead!");
                return;
            }
            
            IsGameOver = true;
            GameOver?.Invoke(false);
            SceneManager.LoadScene(3);
        }
    }
}
