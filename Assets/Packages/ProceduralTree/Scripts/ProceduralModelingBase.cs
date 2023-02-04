using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
	public abstract class ProceduralModelingBase : MonoBehaviour {
		[SerializeField] private bool _buildOnStart;

		public MeshFilter Filter {
			get {
				if(filter == null) {
					filter = GetComponent<MeshFilter>();
				}
				return filter;
			}
		}

		MeshFilter filter;

		protected virtual void Start () {
			if(_buildOnStart)
				Rebuild();
		}

		public void Rebuild() {
			BuildTree((mesh) =>
			{
				if (Filter.sharedMesh != null)
				{
					if (Application.isPlaying)
					{
						Destroy(Filter.sharedMesh);
					}
					else
					{
						DestroyImmediate(Filter.sharedMesh);
					}
				}

				Filter.sharedMesh = mesh;
			});
		}

		protected abstract Mesh Build();

		public abstract TreeBranch BuildData();
		public abstract void BuildTree(System.Action<Mesh> cb);

	}
		
}

