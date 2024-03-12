using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public GameObject lbl_interact;

    public Transform interactorSource;
    public float interactRange;

    public PlayerInput playerInput;
    public InputAction interactAction;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions.FindAction("interact");
        lbl_interact.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo1, interactRange))
        {
            if (hitInfo1.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                lbl_interact.SetActive(true);
            }
            else
            {
                lbl_interact.SetActive(false);
            }
        }

        if (interactAction.IsPressed())
        {
            if(Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    Debug.Log("collided");
                    interactObj.Interact();
                }
            }
        }
    }
}
