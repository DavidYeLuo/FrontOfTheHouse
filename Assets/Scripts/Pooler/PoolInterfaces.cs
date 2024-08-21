using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool {
public interface IPool {
  public void Pool(PoolObject obj);
  public PoolObject Spawn();
}
}
