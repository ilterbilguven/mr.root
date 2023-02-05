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
		private Renderer _renderer;
		
		private List<Tweener> _tweens = new List<Tweener>();

		private Transform _target;
		private float _duration = 4f;

		private void Awake()
		{
			TryGetComponent(out _tree);
			TryGetComponent(out _renderer);
        
		}

		public void Initialize(Transform target, float duration)
		{
			_target = target;
        
			_tree.Data.targetPoint = target;
			
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
				if (_target.TryGetComponent(out Building building))
				{
					building.Capture();
				}
			}));
		}

		public void Cut()
		{
			foreach (var tweener in _tweens)
			{
				tweener.Kill();
			}
		}
	}
}
