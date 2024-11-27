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

  [Header("Dependency")]
  public GameObject Visual;

  private void Start() {
    if (useEditorScale)
      return;
    HoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    OriginalScale = transform.localScale;
  }

  public void OnHoverEnter() { _OnHoverEnter?.Invoke(); }
  public void OnHoverExit() { _OnHoverExit?.Invoke(); }

  // OnHover events common functions below

  public void SetToHoverScale() { Visual.transform.localScale = HoverScale; }

  public void ResetScale() { Visual.transform.localScale = OriginalScale; }
}
}
