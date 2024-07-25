using UnityEngine;
using PlayerAction;
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
  public GameObject unsortedObject;
  public GameObject sortedObject;
  public float secondsToSort;

  private void Start() {
    if (startAsSorted) {
      SetToSorted();
    } else {
      SetToUnsorted();
    }
  }

  public bool IsSorted() { return state == BoxState.SORTED; }
  public void Sort() { SetToSorted(); }
  public void Unsort() { SetToUnsorted(); }
  public void Accept(IInteractor interactor) { interactor.Interact(this); }

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
