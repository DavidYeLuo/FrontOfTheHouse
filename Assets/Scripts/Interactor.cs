using UnityEngine;
using Interactable;

namespace PlayerAction
{
    // Visitor in the Visitor Pattern
    public interface IInteractor
    {
        public void Interact(UtensilBox util);
    }

    // Element in the Visitor Pattern
    public interface IInteractable
    {
        public void Accept(IInteractor interactor);
    }
}
