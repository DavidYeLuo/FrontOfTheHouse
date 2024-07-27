using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: consider naming the namespace. Maybe group it with interactable
namespace ObjectDetection {
public delegate void ObjectLeft(GameObject obj);
public class NotifyOnLeaveTrigger : MonoBehaviour {
  public ObjectLeft objectLeftHandler;
  private void OnTriggerExit(Collider collider) {
    objectLeftHandler?.Invoke(collider.gameObject);
    Debug.Log(collider.gameObject + " obj");
  }
}

}
