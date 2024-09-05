using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity {
public interface IPickupItem {
  public GameObject PickupItem();
  public float GetZOffset() { return 0.0f; }
}
}
