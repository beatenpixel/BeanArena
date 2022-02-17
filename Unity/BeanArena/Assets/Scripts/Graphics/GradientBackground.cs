using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientBackground : MonoBehaviour {

	public Transform t;
	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	public List<GradientPoint> gradientPoints;
	public int orderInLayer;

	private Timer updateTimer;

	private List<Vector3> verts = new List<Vector3>();
	private List<Vector2> uvs0 = new List<Vector2>();
	private List<Color> colors = new List<Color>();
	private List<int> tris = new List<int>();

	private Mesh mesh;

	internal void Init() {

	}

	private void Awake() {
		updateTimer = new Timer(0.1f);
		GenerateBackground();
	}

	private void Update() {
		
        if(updateTimer) {
			updateTimer.AddFromNow();

            UpdateColors();
        }		
	}

    private void UpdateColors() {
        for (int i = 0; i < gradientPoints.Count; i++) {
            colors[i*2] = gradientPoints[i].color;
            colors[i*2+1] = gradientPoints[i].color;
        }

        mesh.SetColors(colors);
    }

	private void GenerateBackground() {
		verts = new List<Vector3>();
		uvs0 = new List<Vector2>();
		colors = new List<Color>();
		tris = new List<int>();

		for (int i = 0; i < gradientPoints.Count; i++) {
			int vertInd = verts.Count - 2;

			verts.Add(new Vector3(-0.5f, gradientPoints[i].position - 0.5f, 0));
			verts.Add(new Vector3(0.5f, gradientPoints[i].position - 0.5f, 0));
			colors.Add(gradientPoints[i].color);
			colors.Add(gradientPoints[i].color);
			uvs0.Add(new Vector2(0, gradientPoints[i].position));
			uvs0.Add(new Vector2(1, gradientPoints[i].position));

			if (i == 0) {

			} else {
				tris.Add(vertInd); tris.Add(vertInd + 2); tris.Add(vertInd + 3);
				tris.Add(vertInd); tris.Add(vertInd + 3); tris.Add(vertInd + 1);
			}
		}

		mesh = new Mesh();
		mesh.SetVertices(verts);
		mesh.SetColors(colors);
		mesh.SetUVs(0, uvs0);
		mesh.SetTriangles(tris, 0);
		meshFilter.mesh = mesh;

		meshRenderer.sortingOrder = orderInLayer;
	}

	public void AlignSizeToCamera(Camera cam) {
		t.localScale = new Vector3(MCamera.SCREEN_WH_RATIO * cam.orthographicSize * 2, cam.orthographicSize * 2) * 1.05f;
	}

	[System.Serializable]
	public struct GradientPoint {
		public float position;
		public Color color;
	}

}

