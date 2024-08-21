using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool {
public class Pooler : IPool {
  private int size;
  private Stack<PoolObject> pool;
  private PoolObject originalObjRef;

  public Pooler(int size, PoolObject obj) {
    originalObjRef = obj;
    pool = new Stack<PoolObject>();
    for (int i = 0; i < size; i++) {
      PoolObject newObj = InstantiateObject();
      newObj.gameObject.SetActive(false);
      newObj.pool = this;
      pool.Push(newObj);
    }
  }
  public void Pool(PoolObject obj) {
    pool.Push(obj);
    obj.gameObject.SetActive(false);
  }
  public PoolObject Spawn() {
    PoolObject obj = pool.Count == 0 ? InstantiateObject() : pool.Pop();
    obj.gameObject.SetActive(true);
    return obj;
  }

  private PoolObject InstantiateObject() {
    PoolObject obj = MonoBehaviour.Instantiate(originalObjRef);
    obj.pool = this;
    return obj;
  }
}
}
