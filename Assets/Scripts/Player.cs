using UnityEngine;
using PlayerAction;
using Interactable;

namespace Player {
public delegate void PauseHandler();
public class Player : MonoBehaviour, IInteractor {
  public PauseHandler pauseHandler;
  [SerializeField]
  private float force;
  [SerializeField]
  private Rigidbody rb;
  private void Start() { rb = GetComponent<Rigidbody>(); }
  private void FixedUpdate() {
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    Vector3 h_vector = horizontal * Vector3.right;
    Vector3 v_vector = vertical * Vector3.forward;

    rb.AddForce(
        force * (h_vector + v_vector),
        ForceMode.Force); // Force Implementation
                          // transform.position += force * (h_vector +
                          // v_vector); // Transform Movement Implementation

    // Interact Button
    RaycastHit hit;
    float INTERACTION_RANGE = 3.0f;
    if (Input.GetKeyUp(KeyCode.J)) {
      if (Physics.Raycast(transform.position,
                          transform.position +
                              transform.forward * INTERACTION_RANGE,
                          out hit, INTERACTION_RANGE)) {
        Debug.DrawLine(transform.position,
                       transform.position + transform.forward * hit.distance,
                       Color.green);
        Debug.Log("[RayCast, Player] Player raycast hit something.");

        {
          IInteractable interactable =
              hit.collider.GetComponent<IInteractable>();
          if (interactable != null)
            interactable.Accept(this);
        }
      } else {
        Debug.DrawLine(transform.position,
                       transform.position +
                           transform.forward * INTERACTION_RANGE,
                       Color.red);
      }
    }
  }
  private void Update() {
    // NOTE: The reason why we use Update instead of FixedUpdate is because
    // fixed update doesn't run when the game is paused.
    if (Input.GetKeyUp(KeyCode.Escape)) {
      pauseHandler.Invoke();
    }
  }
  public void Interact(UtensilBox util) {
    Debug.Log("[Interact, Player] Player interacted with utensil box.");
    util.Sort();
  }
}
}
