using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomShader {
public class SmoothMeshGenerator : MonoBehaviour {
  [SerializeField]
  private Mesh mesh;
  [SerializeField]
  private List<Vector3> v;
  [SerializeField]
  private int neighborsCount;

  private void Start() {
    MeshFilter meshRenderer = GetComponent<MeshFilter>();
    mesh = meshRenderer.mesh;

    List<Vector3> vertices = new List<Vector3>();
    mesh.GetVertices(vertices);

    Dictionary<Vector3, List<int>> neighbors =
        new Dictionary<Vector3, List<int>>();
    for (int i = 0; i < vertices.Count; i++) {
      List<int> temp;
      Debug.Log("Passd?1");
      if (!neighbors.TryGetValue(vertices[i], out temp)) {
        neighbors[vertices[i]] = new List<int>();
      }
      neighbors[vertices[i]].Add(i);
    }
    Debug.Log("Passd?2");
    neighborsCount = neighbors.Count; // Debug

    List<int> indeces;
    List<Vector3> newNormals = new List<Vector3>();
    List<Vector3> objNormals = new List<Vector3>();
    mesh.GetNormals(objNormals);

    for (int i = 0; i < vertices.Count; i++) {
      if (!neighbors.TryGetValue(vertices[i], out indeces)) {
        newNormals.Add(objNormals[i]);
        return;
      }
      Vector3 newNormal = Vector3.zero;
      foreach (int normal in indeces) {
        newNormal += objNormals[normal];
      }
      newNormals.Add(newNormal / indeces.Count);
    }

    mesh.SetNormals(newNormals);
    v = vertices; // Debug
  }
  public static void SmoothMesh(MeshFilter mfilter) {
    Mesh mesh = mfilter.mesh;
    List<Vector3> vertices = new List<Vector3>();
    mesh.GetVertices(vertices);

    Dictionary<Vector3, List<int>> neighbors =
        new Dictionary<Vector3, List<int>>();
    for (int i = 0; i < vertices.Count; i++) {
      List<int> temp;
      if (!neighbors.TryGetValue(vertices[i], out temp)) {
        neighbors[vertices[i]] = new List<int>();
      }
      neighbors[vertices[i]].Add(i);
    }

    List<int> indeces;
    List<Vector3> newNormals = new List<Vector3>();
    List<Vector3> objNormals = new List<Vector3>();
    mesh.GetNormals(objNormals);

    for (int i = 0; i < vertices.Count; i++) {
      if (!neighbors.TryGetValue(vertices[i], out indeces)) {
        newNormals.Add(objNormals[i]);
        return;
      }
      Vector3 newNormal = Vector3.zero;
      foreach (int normal in indeces) {
        newNormal += objNormals[normal];
      }
      newNormals.Add(newNormal / indeces.Count);
    }

    mesh.SetNormals(newNormals);
  }
}
}
