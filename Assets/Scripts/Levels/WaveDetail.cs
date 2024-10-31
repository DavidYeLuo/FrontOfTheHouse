using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level {
[CreateAssetMenu(fileName = "Wave", menuName = "Level/Wave")]
public class WaveDetail : ScriptableObject {
  [field:SerializeField]
  public string label { get; private set; }
  [field:SerializeField]
  public Color UIColor {
    get; private set;
  }
  [field:SerializeField]
  public int NumberOfGuestSpawns {
    get; private set;
  }
  [field:SerializeField]
  public float DurationInSeconds {
    get; private set;
  }
  [field:SerializeField]
  public AnimationCurve SpawnDistribution {
    get; private set;
  }
  // Temporary for now since we only have one kind of guest
  [field:SerializeField]
  public GameObject GuestPrefab {
    get; private set;
  }
  [field:SerializeField]
  public float WaitTime {
    get; private set;
  }
}
}
