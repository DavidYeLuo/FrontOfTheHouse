using UnityEngine;
using PlayerAction;
namespace Interactable
{
    public class UtensilBox : MonoBehaviour, IInteractable
    {
        // TODO: Indicate that the object has changed state
        // Currently, the box looks the same when it is sorted or not
        public enum BoxState { SORTED, UNSORTED }
        public BoxState state = BoxState.UNSORTED;

        public void Sort() { state = BoxState.SORTED; }
        public void Unsort() { state = BoxState.UNSORTED; }
        public void Accept(IInteractor interactor) { interactor.Interact(this); }
    }
}
