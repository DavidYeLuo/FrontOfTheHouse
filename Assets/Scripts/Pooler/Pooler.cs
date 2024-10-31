using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool {
public class Pooler : IPool {
  public int size {
    get { return pool.Count; }
  }
  public int capacity { get; private set; }
  private Stack<PoolObject> pool;
  private PoolObject originalObjRef;

  public Pooler(int size, PoolObject obj) {
    originalObjRef = obj;
    this.capacity = size;
    pool = new Stack<PoolObject>();
    for (int i = 0; i < capacity; i++) {
      PoolObject newObj = InstantiateObject();
      newObj.gameObject.SetActive(false);
      newObj.pool = this;
      pool.Push(newObj);
    }
  }
  public void Pool(PoolObject obj) {
    // Destroy object when the pool size is too big
    if (size >= capacity) {
      // NOTE: Untested code
      MonoBehaviour.Destroy(obj);
      return;
    }
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
