using System;
using System.Collections.Generic;
using DG.Tweening;
using ProceduralModeling;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
namespace MrRoot.Root
{
	public class EnemyRoot : MonoBehaviour
	{
		private ProceduralTree _tree;
		public Renderer _renderer;
		private Transform _target;
		private List<Tweener> _tweens = new List<Tweener>();
		private float _duration;

		public static event Action RootCut;
		public float transitionValue;
		public bool far;

		[SerializeField] private float _rollbackDuration;

		private void Awake()
		{
			TryGetComponent(out _tree);
			TryGetComponent(out _renderer);
        
		}

		public void Initialize(Transform target, float duration)
		{
			_target = target;
			_tree.Data.targetPoint = target;
			_duration = duration;
			_tree.Data.randomSeed = Random.Range(0, int.MaxValue);
			_tree.Rebuild();
			
			Animate();
		}
		
		//seed: first 
		//target bias: runtime
		//length att
		// radius - length - radius att
		// bend degree - az
		
		[Button]
		public void Animate()
		{
			// _tweens.Add(DOVirtual.Float(0, 1, 2f, value =>
			// {
			// 	_tree.Data.targetBias = value;
			// 	_tree.Rebuild();
			// }));
			//
			// _tweens.Add(DOVirtual.Float(0.5f, 4, 2f, value =>
			// {
			// 	_tree.length = value;
			// }));

			_tweens.Add(_renderer.material.DOFloat(1f, "_Transition", _duration).From(0f).OnComplete(() =>
			{
				if (!_target) return;
				if (_target.TryGetComponent(out Building building))
				{
					building.Capture();
				}
			}));
		}
		public void RollBack(float start, float end)
		{
			foreach (var rendererMaterial in  _renderer.materials)
			{
				rendererMaterial.DOFloat(end, "_Transition", _rollbackDuration).From(start);
			}
		}

		public void DestroyedObject(GameObject obje)
		{
			obje.transform.DOLocalMoveY(-10f, 2f);
		}

		public void Cut()
		{
			foreach (var tweener in _tweens)
			{
				tweener.Kill();
			}
			
			RootCut?.Invoke();
		}
	}
}
