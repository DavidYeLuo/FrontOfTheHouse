using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool {
public class PoolObject : MonoBehaviour {
  public IPool pool;
  public void Destroy() { pool.Pool(this); }
}
}
