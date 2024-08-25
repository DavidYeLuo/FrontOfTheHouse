using UnityEngine;
using PlayerAction;
using ObjectDetection;
using ObjectPool;
using UnityEngine.Assertions;
namespace Interactable {
public delegate void UtensilHandler(bool isSorted);
public class UtensilBox : MonoBehaviour, IInteractable {
  public event UtensilHandler isUtensilBoxSortedHandler;
  public enum BoxState { SORTED, UNSORTED }

  private bool startAsSorted;
  [SerializeField,
   Tooltip(
       "Note: Don't change state in the editor. Use the startAsSorted flag.")]
  private BoxState state;
  [SerializeField]
  private GameObject goldenSpoon;
  public GameObject unsortedObject;
  public GameObject sortedObject;
  public float secondsToSort;
  public int startingPoolSize;

  private static Pooler pooler;
  private PoolObject poolGoldenSpoon;

  private void Start() {
    if (startAsSorted) {
      SetToSorted();
    } else {
      SetToUnsorted();
    }
  }
  private void Awake() {
    poolGoldenSpoon = goldenSpoon.GetComponent<PoolObject>();
    Assert.IsNotNull(poolGoldenSpoon); // Fails when golden spoon doesn't have a
    if (pooler == null)
      pooler = new Pooler(startingPoolSize, poolGoldenSpoon);
  }

  public bool IsSorted() { return state == BoxState.SORTED; }
  public void Sort() { SetToSorted(); }
  public void Unsort() { SetToUnsorted(); }
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
  public PoolObject GetGoldenSpoon() { return pooler.Spawn(); }

  private void SetToSorted() {
    state = BoxState.SORTED;
    sortedObject.SetActive(true);
    unsortedObject.SetActive(false);
    isUtensilBoxSortedHandler?.Invoke(true);
  }
  private void SetToUnsorted() {
    state = BoxState.UNSORTED;
    sortedObject.SetActive(false);
    unsortedObject.SetActive(true);
    isUtensilBoxSortedHandler?.Invoke(false);
  }
}
}
