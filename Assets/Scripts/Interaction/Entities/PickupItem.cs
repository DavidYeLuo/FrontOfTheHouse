using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity {
public interface IPickupItem {
  public GameObject GetGameObject();
  public float GetZOffset() { return 0.0f; }
}
}
