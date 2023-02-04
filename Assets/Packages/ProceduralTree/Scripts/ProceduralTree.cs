﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ProceduralModeling {
	public struct MeshData
	{
		public Vector3[] vertices, normals;
		public Vector4[] tangents;
		public Vector2[] uvs;
		public int[] triangles;
	}

	public class ProceduralTree : ProceduralModelingBase {

		public event Action Rebuilt;
		public TreeData Data { get { return data; } }
		[SerializeField] TreeData data;
		[SerializeField, Range(2, 8)] protected int generations = 5;
		[SerializeField, Range(0.5f, 5f)] protected float length = 1f;
		[SerializeField, Range(0.1f, 2f)] protected float radius = 0.15f;
		[SerializeField] private MeshCollider _col;

		private TreeBranch _treeRoot;
		private Coroutine _buildRoutine;
		public TreeBranch TreeRoot => _treeRoot;

		const float PI2 = Mathf.PI * 2f;
		
		void Update()
		{
			Rebuild();
			data.targetPoint.position = Vector3.Lerp(Vector3.zero, new Vector3(5, 0, 5), Mathf.Abs(Mathf.Sin(Time.time)));
		}
		public override void BuildTree(Action<Mesh> cb)
		{
			if(_buildRoutine == null)
			{
				data.tempTargetPosition = data.targetPoint.position;
				_buildRoutine = StartCoroutine(BuildTreeRoutine((mesh) =>
				{
					cb?.Invoke(mesh);
					Rebuilt?.Invoke();
					_col.sharedMesh = Filter.sharedMesh;
				}));
			}
		}
		
		public IEnumerator BuildTreeRoutine(Action<Mesh> postGen)
		{
			bool genFinished = false;
			MeshData meshData = new MeshData();
			
			Task<MeshData>.Run(() => BuildAsync(data, generations, length, radius, true)).ContinueWith((task) => 
			{
				genFinished = true;
				meshData = task.Result;
			});

			yield return new WaitUntil(() => genFinished);
			
			Mesh mesh = null;

			mesh = new Mesh();
			mesh.vertices = meshData.vertices;
			mesh.normals = meshData.normals;
			mesh.tangents = meshData.tangents;
			mesh.uv = meshData.uvs;
			mesh.triangles = meshData.triangles;
			_buildRoutine = null;
			postGen?.Invoke(mesh);
		}

		public async static Task<MeshData> BuildAsync(TreeData data, int generations, float length, float radius, bool meshGen) {
			Thread.CurrentThread.Name = "ProcTreeBuilder";
			data.Setup();

			var root = new TreeBranch(
				generations, 
				length, 
				radius, 
				data
			);

			List<Vector3> vertices = null, normals = null;
			List<Vector4> tangents = null;
			List<Vector2> uvs = null;
			List<int> triangles = null;
			
			if (meshGen)
			{
				vertices = new List<Vector3>();
				normals = new List<Vector3>();
				tangents = new List<Vector4>();
				uvs = new List<Vector2>();
				triangles = new List<int>();
			}
			
			float maxLength = TraverseMaxLength(root);

			Traverse(root, (branch) => {
				var offset = 0;
				if (meshGen)
				{
					offset = vertices.Count;
				}

				var vOffset = branch.Offset / maxLength;
				var vLength = branch.Length / maxLength;

				for(int i = 0, n = branch.Segments.Count; i < n; i++) {
					var t = 1f * i / (n - 1);
					var v = vOffset + vLength * t;

					var segment = branch.Segments[i];
					var N = segment.Frame.Normal;
					var B = segment.Frame.Binormal;

					if (meshGen)
					{
						for (int j = 0; j <= data.radialSegments; j++)
						{
							// 0.0 ~ 2π
							var u = 1f * j / data.radialSegments;
							float rad = u * PI2;

							float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
							var normal = (cos * N + sin * B).normalized;
							vertices.Add(segment.Position + segment.Radius * normal);
							normals.Add(normal);

							var tangent = segment.Frame.Tangent;
							tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));

							uvs.Add(new Vector2(u, v));
						}
					}
				}

				if (meshGen)
				{
					for (int j = 1; j <= data.heightSegments; j++)
					{
						for (int i = 1; i <= data.radialSegments; i++)
						{
							int a = (data.radialSegments + 1) * (j - 1) + (i - 1);
							int b = (data.radialSegments + 1) * j + (i - 1);
							int c = (data.radialSegments + 1) * j + i;
							int d = (data.radialSegments + 1) * (j - 1) + i;

							a += offset;
							b += offset;
							c += offset;
							d += offset;

							triangles.Add(a); triangles.Add(d); triangles.Add(b);
							triangles.Add(b); triangles.Add(d); triangles.Add(c);
						}
					}
				}
			});

			MeshData meshData = new MeshData();

			meshData.vertices = vertices.ToArray();
			meshData.normals = normals.ToArray();
			meshData.tangents = tangents.ToArray();
			meshData.uvs = uvs.ToArray();
			meshData.triangles = triangles.ToArray();

			return meshData;
		}

		public static Mesh Build(TreeData data, int generations, float length, float radius, bool meshGen, out TreeBranch root) {
			data.Setup();

			root = new TreeBranch(
				generations, 
				length, 
				radius, 
				data
			);

			List<Vector3> vertices = null, normals = null;
			List<Vector4> tangents = null;
			List<Vector2> uvs = null;
			List<int> triangles = null;
			
			if (meshGen)
			{
				vertices = new List<Vector3>();
				normals = new List<Vector3>();
				tangents = new List<Vector4>();
				uvs = new List<Vector2>();
				triangles = new List<int>();
			}
			
			float maxLength = TraverseMaxLength(root);

			Traverse(root, (branch) => {
				var offset = 0;
				if (meshGen)
				{
					offset = vertices.Count;
				}

				var vOffset = branch.Offset / maxLength;
				var vLength = branch.Length / maxLength;

				for(int i = 0, n = branch.Segments.Count; i < n; i++) {
					var t = 1f * i / (n - 1);
					var v = vOffset + vLength * t;

					var segment = branch.Segments[i];
					var N = segment.Frame.Normal;
					var B = segment.Frame.Binormal;

					if (meshGen)
					{
						for (int j = 0; j <= data.radialSegments; j++)
						{
							// 0.0 ~ 2π
							var u = 1f * j / data.radialSegments;
							float rad = u * PI2;

							float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
							var normal = (cos * N + sin * B).normalized;
							vertices.Add(segment.Position + segment.Radius * normal);
							normals.Add(normal);

							var tangent = segment.Frame.Tangent;
							tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));

							uvs.Add(new Vector2(u, v));
						}
					}
				}

				if (meshGen)
				{
					for (int j = 1; j <= data.heightSegments; j++)
					{
						for (int i = 1; i <= data.radialSegments; i++)
						{
							int a = (data.radialSegments + 1) * (j - 1) + (i - 1);
							int b = (data.radialSegments + 1) * j + (i - 1);
							int c = (data.radialSegments + 1) * j + i;
							int d = (data.radialSegments + 1) * (j - 1) + i;

							a += offset;
							b += offset;
							c += offset;
							d += offset;

							triangles.Add(a); triangles.Add(d); triangles.Add(b);
							triangles.Add(b); triangles.Add(d); triangles.Add(c);
						}
					}
				}
			});

			Mesh mesh = null;

			if (meshGen)
			{
				mesh = new Mesh();
				mesh.vertices = vertices.ToArray();
				mesh.normals = normals.ToArray();
				mesh.tangents = tangents.ToArray();
				mesh.uv = uvs.ToArray();
				mesh.triangles = triangles.ToArray();
			}
			return mesh;
		}

		public override TreeBranch BuildData ()
		{
			TreeBranch root;
			Build(data, generations, length, radius, false, out root);

			Debug.Log($"root: {root.Children.Count}");
			return root;
		}

		protected override Mesh Build ()
		{
			TreeBranch root;
			var mesh = Build(data, generations, length, radius, true, out root);

			_treeRoot = root;

			Debug.Log($"root: {root.Children.Count}");
			return mesh;
		}

		static float TraverseMaxLength(TreeBranch branch) {
			float max = 0f;
			branch.Children.ForEach(c => {
				max = Mathf.Max(max, TraverseMaxLength(c));
			});
			return branch.Length + max;
		}

		static void Traverse(TreeBranch from, Action<TreeBranch> action) {
			if(from.Children.Count > 0) {
				from.Children.ForEach(child => {
					Traverse(child, action);
				});
			}
			action(from);
		}

	}

	[System.Serializable]
	public class TreeData {
		public int randomSeed = 0;
		[Range(0.25f, 0.95f)] public float lengthAttenuation = 0.8f, radiusAttenuation = 0.5f;
		[Range(1, 3)] public int branchesMin = 1, branchesMax = 3;
        [Range(-45f, 0f)] public float growthAngleMin = -15f;
        [Range(0f, 45f)] public float growthAngleMax = 15f;
        [Range(1f, 10f)] public float growthAngleScale = 4f;
        [Range(0f, 45f)] public float branchingAngle = 15f;
		[Range(4, 20)] public int heightSegments = 10, radialSegments = 8;
		[Range(0.0f, 0.35f)] public float bendDegree = 0.1f;
		public Transform targetPoint;
		public Vector3 tempTargetPosition;
		[Range(0f, 1f)] public float targetBias;
		Rand rnd;

		public void Setup() {
			rnd = new Rand(randomSeed);
		}

		public int Range(int a, int b) {
			return rnd.Range(a, b);
		}

		public float Range(float a, float b) {
			return rnd.Range(a, b);
		}

		public int GetRandomBranches() {
			return rnd.Range(branchesMin, branchesMax + 1);
		}

		public float GetRandomGrowthAngle() {
			return rnd.Range(growthAngleMin, growthAngleMax);
		}

		public float GetRandomBendDegree() {
			return rnd.Range(-bendDegree, bendDegree);
		}
	}

	public class TreeBranch {
		public int Generation { get { return generation; } }
		public List<TreeSegment> Segments { get { return segments; } }
		public List<TreeBranch> Children { get { return children; } }

		public Vector3 From { get { return from; } }
		public Vector3 To { get { return to; } }
		public float Length { get { return length; } } 
		public float Offset { get { return offset; } }

		int generation;

		List<TreeSegment> segments;
		List<TreeBranch> children;

		Vector3 from, to;
		float fromRadius, toRadius;
		float length;
		float offset;

		// for Root branch constructor
		public TreeBranch(int generation, float length, float radius, TreeData data) : this(new List<TreeBranch>(), generation, generation, Vector3.zero, Vector3.up, Vector3.right, Vector3.back, length, radius, 0f, data) {
		}

		protected TreeBranch(List<TreeBranch> branches, int generation, int generations, Vector3 from, Vector3 tangent, Vector3 normal, Vector3 binormal, float length, float radius, float offset, TreeData data) {
			this.generation = generation;

			this.fromRadius = radius;
			this.toRadius = (generation == 0) ? 0f : radius * data.radiusAttenuation;

			this.from = from;
			var direction = (data.tempTargetPosition - from).normalized;
            var scale = Mathf.Lerp(1f, data.growthAngleScale, 1f - 1f * generation / generations);
            // var rotation = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), normal) * Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), binormal);
            var rotation = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), normal) * Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), binormal);

			var dx = Vector3.Lerp(rotation * tangent, direction, data.targetBias);
			dx.Normalize();

			this.to = from + dx * length;


            this.length = length;
			this.offset = offset;

			segments = BuildSegments(data, fromRadius, toRadius, normal, binormal);

            branches.Add(this);

			children = new List<TreeBranch>();
			if(generation > 0) {
				int count = data.GetRandomBranches();
				for(int i = 0; i < count; i++) {
                    float ratio;
                    if(count == 1)
                    {
                        // for zero division
                        ratio = 1f;
                    } else
                    {
                        ratio = Mathf.Lerp(0.5f, 1f, (1f * i) / (count - 1));
                    }

                    var index = Mathf.FloorToInt(ratio * (segments.Count - 1));
					var segment = segments[index];

                    Vector3 nt, nn, nb;
                    if(ratio >= 1f)
                    {
                        // sequence branch
                        nt = segment.Frame.Tangent;
                        nn = segment.Frame.Normal;
                        nb = segment.Frame.Binormal;
                    } else
                    {
                        var phi = Quaternion.AngleAxis(i * 90f, tangent);
                        // var psi = Quaternion.AngleAxis(data.branchingAngle, normal) * Quaternion.AngleAxis(data.branchingAngle, binormal);
                        var psi = Quaternion.AngleAxis(data.branchingAngle, normal);
                        var rot = phi * psi;
                        nt = rot * tangent;
                        nn = rot * normal;
                        nb = rot * binormal;
                    }

					var child = new TreeBranch(
                        branches,
						this.generation - 1, 
                        generations,
						segment.Position, 
						nt, 
						nn, 
						nb, 
						length * Mathf.Lerp(1f, data.lengthAttenuation, ratio), 
						radius * Mathf.Lerp(1f, data.radiusAttenuation, ratio),
						offset + length,
						data
					);

					children.Add(child);
				}
			}
		}

		List<TreeSegment> BuildSegments (TreeData data, float fromRadius, float toRadius, Vector3 normal, Vector3 binormal) {
			var segments = new List<TreeSegment>();

			var points = new List<Vector3>();

			var length = (to - from).magnitude;
			var bend = length * (normal * data.GetRandomBendDegree() + binormal * data.GetRandomBendDegree());
			points.Add(from);
			points.Add(Vector3.Lerp(from, to, 0.25f) + bend);
			points.Add(Vector3.Lerp(from, to, 0.75f) + bend);
			points.Add(to);

			var curve = new CatmullRomCurve(points);

			var frames = curve.ComputeFrenetFrames(data.heightSegments, normal, binormal, false);
			for(int i = 0, n = frames.Count; i < n; i++) {
				var u = 1f * i / (n - 1);
                var radius = Mathf.Lerp(fromRadius, toRadius, u);

				var position = curve.GetPointAt(u);
				var segment = new TreeSegment(frames[i], position, radius);
				segments.Add(segment);
			}
			return segments;
		}

	}

	public class TreeSegment {
		public FrenetFrame Frame { get { return frame; } }
		public Vector3 Position { get { return position; } }
        public float Radius { get { return radius; } }

		FrenetFrame frame;
		Vector3 position;
        float radius;

		public TreeSegment(FrenetFrame frame, Vector3 position, float radius) {
			this.frame = frame;
			this.position = position;
            this.radius = radius;
		}
	}

	public class Rand {
		System.Random rnd;

		public float value {
			get {
				return (float)rnd.NextDouble();
			}
		}

		public Rand(int seed) {
			rnd = new System.Random(seed);
		}

		public int Range(int a, int b) {
			var v = value;
			return Mathf.FloorToInt(Mathf.Lerp(a, b, v));
		}

		public float Range(float a, float b) {
			var v = value;
			return Mathf.Lerp(a, b, v);
		}
	}

}

