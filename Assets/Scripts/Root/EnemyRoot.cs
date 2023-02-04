using System;
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

		private void Awake()
		{
			TryGetComponent(out _tree);
		}

		public void Initialize(Transform target)
		{
			_tree.Data.targetPoint = target;
			//_tree.Rebuild();
			
			_tree.Data.randomSeed = Random.Range(0, int.MaxValue);
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
			DOVirtual.Float(0, 1, 1f, value =>
			{
				_tree.Data.targetBias = value;
				_tree.Rebuild();
			});
		}
	}
}
