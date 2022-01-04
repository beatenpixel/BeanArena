using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentTrail : MonoBehaviour {

	public Transform targetT;
	public Vector2 localPointA;
	public Vector2 localPointB;

	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	public int pointsCount = 20;
	public Vector2 tiling = new Vector2(2, 2);
	public AnimationCurve widthCurve;
	public float widthMod = 2;
	public int orderInLayer;

	private List<Vector3> verts = new List<Vector3>();
	private List<Vector4> uvs0 = new List<Vector4>();
	private List<Vector4> uvs1 = new List<Vector4>();
	private List<Color> colors = new List<Color>();
	private List<int> tris = new List<int>();

	private Mesh mesh;

	private Timer updateTimer;

    private void Awake() {
		meshRenderer.sortingOrder = orderInLayer;

		mesh = new Mesh();
		meshFilter.mesh = mesh;
	}

    private void LateUpdate() {
        
    }

}
