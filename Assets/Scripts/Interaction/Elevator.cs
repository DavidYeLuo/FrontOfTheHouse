using UnityEngine;
using Interactable;
namespace PlayerAction {
public class Elevator : MonoBehaviour, IInteractable {
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}
