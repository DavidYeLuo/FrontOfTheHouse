using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Interactable;

namespace Interactable {

public class ItemHoverConfig : ScriptableObject {}
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

  public Material hoverMaterial;
  private List<Material[]> hoverMaterials;
  private List<Material[]> originalMaterials;

  [Header("Dependency")]
  public GameObject Visual;
  [SerializeField]
  private List<Renderer> targetRendererList;

  private void Start() {
    if (useEditorScale)
      return;
    HoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    OriginalScale = transform.localScale;

    originalMaterials = new List<Material[]>();
    hoverMaterials = new List<Material[]>();
    for (int i = 0; i < targetRendererList.Count; i++) {
      Material originalMaterial = targetRendererList[i].material;
      Material copyHoverMaterial = new Material(hoverMaterial);
      copyHoverMaterial.SetColor("_BaseColor", Color.black);
      copyHoverMaterial.SetFloat("_Thickness", 3.0f);

      originalMaterials.Add(new Material[] { originalMaterial });
      hoverMaterials.Add(
          new Material[] { originalMaterial, copyHoverMaterial });
    }
    if (hoverMaterial == null)
      Debug.LogWarning(
          $"Id: {this.gameObject.name}, Highlight material in ItemHover.cs is null. Calling SetHighlight Material is going to be crash.");
  }

  public void OnHoverEnter() { _OnHoverEnter?.Invoke(); }
  public void OnHoverExit() { _OnHoverExit?.Invoke(); }

  // OnHover events common functions below

  public void SetToHoverScale() { Visual.transform.localScale = HoverScale; }

  public void ResetScale() { Visual.transform.localScale = OriginalScale; }
  public void AddOutlineMaterial() {
    for (int i = 0; i < targetRendererList.Count; i++) {
      targetRendererList[i].materials = hoverMaterials[i];
    }
  }
  public void RemoveOutlineMaterial() {
    for (int i = 0; i < targetRendererList.Count; i++) {
      targetRendererList[i].materials = originalMaterials[i];
    }
  }
}
}
