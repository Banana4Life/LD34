using UnityEngine;
using System.Collections;

public class TriangleShape : MonoBehaviour {

    public MeshCollider collider;
	// Use this for initialization
	void Start () {
        Mesh mesh = new Mesh();
        collider.sharedMesh = mesh;
        mesh.vertices = new Vector3[] { new Vector3(-0.5f, -0.35f, 0), new Vector3(0.5f, -0.35f, 0), new Vector3(0, 0.35f, 0) };
        mesh.uv = new Vector2[] { new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 0) };
        mesh.triangles = new int[] { 0, 1, 2 };
    }
}
