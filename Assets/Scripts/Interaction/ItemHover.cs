using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Interactable;
using CustomShader;

namespace Interactable {

public class ItemHoverConfig : ScriptableObject {}
// This game object should be at the top of the hierarchy along with collider
// This is because the player will raycast to try to find the IHover component
// to call OnHover events
public class ItemHover : MonoBehaviour, IHover {
  [SerializeField]
  private UnityEvent _OnHoverEnter;
  [SerializeField]
  private UnityEvent _OnHoverExit;

  public Material hoverMaterial;

  [Header("Dependency")]
  public GameObject Visual;
  [SerializeField]
  private List<Renderer> targetRendererList;

  private List<GameObject> outlineObjects;
  private GameObject outlineObject;

  private void Start() {
    // for (int i = 0; i < targetRendererList.Count; i++) {
    //   GameObject newObj =
    //       new GameObject($"{targetRendererList[i].name}_outline");
    //   newObj.transform.SetParent(targetRendererList[i].gameObject.transform);
    //   newObj.transform.localPosition = Vector3.zero;
    //   newObj.transform.localRotation = Quaternion.identity;
    //   newObj.transform.localScale = Vector3.one;
    //
    //   outlineObjects.Add(newObj);
    //
    //   MeshFilter mfilter = newObj.AddComponent<MeshFilter>();
    //   mfilter.mesh =
    //       targetRendererList[i].gameObject.GetComponent<MeshFilter>().mesh;
    //   SmoothMeshGenerator.SmoothMesh(
    //       mfilter); // Using the inverted hull method, this will prevent
    //       sharp
    //                 // objects to have artifacts
    //   MeshRenderer mRenderer = newObj.AddComponent<MeshRenderer>();
    //   mRenderer.material = hoverMaterial;
    //   newObj.SetActive(false);
    // }
    MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    CombineInstance[] combine = new CombineInstance[meshFilters.Length];
    for (int i = 0; i < meshFilters.Length; i++) {
      combine[i].mesh = meshFilters[i].sharedMesh;
      combine[i].transform = transform.worldToLocalMatrix *
                             meshFilters[i].transform.localToWorldMatrix;
    }
    Mesh mesh = new Mesh();
    mesh.CombineMeshes(combine);

    outlineObject = new GameObject("Outline Object");
    outlineObject.transform.SetParent(this.transform);
    outlineObject.transform.localPosition = Vector3.zero;
    outlineObject.transform.localRotation = Quaternion.identity;
    outlineObject.transform.localScale = Vector3.one;
    MeshFilter mFilter = outlineObject.AddComponent<MeshFilter>();
    MeshRenderer mRenderer = outlineObject.AddComponent<MeshRenderer>();
    mFilter.mesh = mesh;
    SmoothMeshGenerator.SmoothMesh(mFilter); // Using the inverted hull method,
                                             // this will prevent sharp

    mRenderer.material = hoverMaterial;

    if (hoverMaterial == null)
      Debug.LogWarning(
          $"Id: {this.gameObject.name}, Highlight material in ItemHover.cs is null. Calling SetHighlight Material is going to be crash.");
  }

  public void OnHoverEnter() { _OnHoverEnter?.Invoke(); }
  public void OnHoverExit() { _OnHoverExit?.Invoke(); }

  // OnHover events common functions below

  public void AddOutlineMaterial() {
    outlineObject.SetActive(true);
    // for (int i = 0; i < outlineObjects.Count; i++) {
    //   outlineObjects[i].SetActive(true);
    // }
  }
  public void RemoveOutlineMaterial() {
    outlineObject.SetActive(false);
    // for (int i = 0; i < targetRendererList.Count; i++) {
    //   outlineObjects[i].SetActive(false);
    // }
  }
  public void AddSelectOutlineLayer() {}
  public void RemoveSelectOutlineLayer() {}
}
}
