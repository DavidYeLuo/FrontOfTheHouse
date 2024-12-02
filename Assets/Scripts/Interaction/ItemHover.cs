using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Interactable;

namespace Interactable {
// This game object should be at the top of the hierarchy along with collider
// This is because the player will raycast to try to find the IHover component
// to call OnHover events
public class ItemHover : MonoBehaviour, IHover {
  [SerializeField]
  private bool useEditorScale = false;
  [Tooltip("Uses scale if useEditorScale is true")]
  public Vector3 HoverScale;
  [Tooltip("Uses scale if useEditorScale is true")]
  public Vector3 OriginalScale;

  [SerializeField]
  private UnityEvent _OnHoverEnter;
  [SerializeField]
  private UnityEvent _OnHoverExit;

  public Material highlightMaterial;
  private List<Material> highlightMaterials;
  private List<Material> originalMaterial;

  [Header("Dependency")]
  public GameObject Visual;
  [SerializeField]
  private List<Renderer> targetRendererList;

  private void Start() {
    if (useEditorScale)
      return;
    HoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    OriginalScale = transform.localScale;

    // TODO: generate a highlight material
    originalMaterial = new List<Material>();
    highlightMaterials = new List<Material>();
    foreach (var targetRenderer in targetRendererList) {
      Color previousColor = targetRenderer.material.color;
      originalMaterial.Add(targetRenderer.material);
      targetRenderer.material = highlightMaterial;
      targetRenderer.material.SetColor("_BaseColor", previousColor);
      // targetRenderer.material.SetFloat("_Thickness", 3.0f);
      highlightMaterials.Add(targetRenderer.material);
    }
    for (int i = 0; i < targetRendererList.Count; i++) {
      targetRendererList[i].material = originalMaterial[i];
    }
    if (highlightMaterial == null)
      Debug.LogWarning(
          $"Id: {this.gameObject.name}, Highlight material in ItemHover.cs is null. Calling SetHighlight Material is going to be crash.");
  }

  public void OnHoverEnter() { _OnHoverEnter?.Invoke(); }
  public void OnHoverExit() { _OnHoverExit?.Invoke(); }

  // OnHover events common functions below

  public void SetToHoverScale() { Visual.transform.localScale = HoverScale; }

  public void ResetScale() { Visual.transform.localScale = OriginalScale; }
  public void SetToHighlightMaterial() {
    for (int i = 0; i < targetRendererList.Count; i++) {
      targetRendererList[i].material = highlightMaterials[i];
    }
  }
  public void SetToOriginalMaterial() {
    for (int i = 0; i < targetRendererList.Count; i++) {
      targetRendererList[i].material = originalMaterial[i];
    }
  }
}
}
