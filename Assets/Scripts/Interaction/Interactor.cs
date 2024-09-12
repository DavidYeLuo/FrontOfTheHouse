using UnityEngine;
using Interactable;

namespace PlayerAction {
// Visitor in the Visitor Pattern
public interface IInteractor {
  public void Interact(UtensilBox util);
  public void Interact(LandfillCan can);
  public void Interact(MuffinBox muffinBox);
  public void Interact(FoodTray foodTray);
  public void Interact(SpeedRack speedRack);
  public void Interact(Elevator elevator);
}

// Element in the Visitor Pattern
public interface IInteractable {
  public void Accept(IInteractor interactor);
}
}
