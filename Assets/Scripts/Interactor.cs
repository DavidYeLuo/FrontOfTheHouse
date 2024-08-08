using UnityEngine;
using Interactable;

namespace PlayerAction {
// Visitor in the Visitor Pattern
public interface IInteractor {
  public void Interact(UtensilBox util);
  public void Interact(LandfillCan can);
  public void Interact(MuffinBox muffinBox);
}

// Element in the Visitor Pattern
public interface IInteractable {
  public void Accept(IInteractor interactor);
}
}
