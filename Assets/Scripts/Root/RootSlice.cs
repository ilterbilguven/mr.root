using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MrRoot.Root
{
	public class RootSlice : BzSliceableObjectBase, IBzSliceableNoRepeat
	{
		[HideInInspector]
		[SerializeField] int _sliceId;
		[HideInInspector]
		[SerializeField] float _lastSliceTime = float.MinValue;

		private float _bound;

		private Collider _meshCollider;
		private Renderer _renderer;
		

		private void Start()
		{
			_meshCollider = GetComponent<Collider>();
			_renderer = GetComponent<Renderer>();
			Results += OnResult;
		}

		private void OnDestroy()
		{
			Results -= OnResult;
		}

		/// <summary>
		/// If your code do not use SliceId, it can relay on delay between last slice and new.
		/// If real delay is less than this value, slice will be ignored
		/// </summary>
		public float delayBetweenSlices = 1f;

		public void Slice(Plane plane, int sliceId, Action<BzSliceTryResult> callBack)
		{
			float currentSliceTime = Time.time;

			// we should prevent slicing same object:
			// - if _delayBetweenSlices was not exceeded
			// - with the same sliceId
			if ((sliceId == 0 & _lastSliceTime + delayBetweenSlices > currentSliceTime) |
				(sliceId != 0 & _sliceId == sliceId))
			{
				return;
			}

			// exit if it have LazyActionRunner
			if (GetComponent<LazyActionRunner>() != null)
				return;

			_lastSliceTime = currentSliceTime;
			_sliceId = sliceId;

			Slice(plane, callBack);
		}

		protected override BzSliceTryData PrepareData(Plane plane)
		{
			// remember some data. Later we could use it after the slice is done.
			// here I add Stopwatch object to see how much time it takes
			// and vertex count to display.
			ResultData addData = new ResultData();

			// count vertices
			var filters = GetComponentsInChildren<MeshFilter>();
			for (int i = 0; i < filters.Length; i++)
			{
				addData.vertexCount += filters[i].sharedMesh.vertexCount;
			}

			// remember start time
			addData.stopwatch = Stopwatch.StartNew();

			// colliders that will be participating in slicing
			var colliders = gameObject.GetComponentsInChildren<Collider>();

			// return data
			return new BzSliceTryData()
			{
				// componentManager: this class will manage components on sliced objects
				componentManager = new StaticComponentManager(gameObject, plane, colliders),
				plane = plane,
				addData = addData,
			};
		}

		protected override void OnSliceFinished(BzSliceTryResult result)
		{
			if (!result.sliced)
				return;

			// on sliced, get data that we saved in 'PrepareData' method
			var addData = (ResultData)result.addData;
			addData.stopwatch.Stop();
			drawText += gameObject.name +
				". VertCount: " + addData.vertexCount.ToString() + ". ms: " +
				addData.stopwatch.ElapsedMilliseconds.ToString() + Environment.NewLine;

			if (drawText.Length > 1500) // prevent very long text
				drawText = drawText.Substring(drawText.Length - 1000, 1000);
		}

		static string drawText = "-";

		void OnGUI()
		{
			GUI.Label(new Rect(10, 10, 2000, 2000), drawText);
		}

		// DTO that we pass to slicer and then receive back
		class ResultData
		{
			public int vertexCount;
			public Stopwatch stopwatch;
		}
		
		public void Slice(Vector3 normal, Vector3 point)
		{
			if (TryGetComponent(out EnemyRoot enemyRoot))
			{
				enemyRoot.Cut();
			}
			
			
			_meshCollider.enabled = true;

			_bound = point.y;
			
			Plane plane = new Plane(normal, point);

			Slice(plane, _sliceID++, null);
			
			
		}

		private void OnResult(BzSliceTryResult result)
		{
			result.outObjectNeg.layer = LayerMask.NameToLayer("Ignore Raycast");
			result.outObjectPos.layer = LayerMask.NameToLayer("Ignore Raycast");
			var rendererNeg = result.outObjectNeg.GetComponent<Renderer>();
			var rendererPos = result.outObjectPos.GetComponent<Renderer>();
			var material = rendererNeg.material;
			result.outObjectPos.GetComponent<Renderer>().materials[0] = new Material(material);

			
			var negCenter = rendererNeg.bounds.center;
			Debug.DrawLine(negCenter, negCenter + Vector3.up, Color.red, 15f);
			var posCenter = rendererPos.bounds.center;
			Debug.DrawLine(posCenter, posCenter + Vector3.up, Color.yellow, 15f);
			var negDist = Vector3.Distance(negCenter,  result.outObjectNeg.transform.position);
			var posDist = Vector3.Distance(posCenter,  result.outObjectPos.transform.position);

			float start = 0f, end = 0f;

			if (negDist < posDist)
			{
				if (result.outObjectNeg.TryGetComponent(out EnemyRoot enemyRoot))
				{
					enemyRoot.RollBack(enemyRoot.transitionValue, 0f);
					enemyRoot.DestroyedObject(result.outObjectPos);
				}
			}
			else
			{
				if (result.outObjectPos.TryGetComponent(out EnemyRoot enemyRoot2))
				{
					enemyRoot2.RollBack(enemyRoot2.transitionValue, 0f);
					enemyRoot2.DestroyedObject(result.outObjectNeg);
				}
			}
		}


		private bool _sliced;
		
		private static int _sliceID;
	}
}
