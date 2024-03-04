using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction flashlightAction;
    InputAction runAction;

    [Category("Movement")]
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject hand;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float gravity = 10f;

    [SerializeField] float lookSpeed = 2f;
    [SerializeField] float lookXLimit = 45f;
    
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    bool canMove = true;
    bool isRunning = false;

    bool flashlightOn = false;
    [SerializeField] GameObject flashlight;

    [Category("Head Bobbing")]
    [SerializeField] float walkingBobbingSpeed = 6f;
    [SerializeField] float runningBobbingSpeed = 10f;
    [SerializeField] float bobbingAmount = 0.05f;

    float defaultPosY = 0;
    float defaultHandPosX = 0;
    float timer = 0;
 
    
    CharacterController characterController;
    void Start()
    {
        // Inputs.
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("movement");
        lookAction = playerInput.actions.FindAction("Look");
        flashlightAction = playerInput.actions.FindAction("toggleFlashlight");
        runAction = playerInput.actions.FindAction("run");

        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultPosY = playerCamera.transform.localPosition.y;
        defaultHandPosX = hand.transform.localPosition.x;
    }
 
    void Update()
    {
        movement();
        toggleFlashlight();
        headBobbing();
    }

    void toggleFlashlight() {
        flashlightOn = flashlightAction.IsPressed() ? !flashlightOn : flashlightOn;

       flashlight.GetComponentInChildren<Light>().enabled = flashlightOn;
    }

    void headBobbing()
    {
        float bobbingSpeed = (isRunning) ? runningBobbingSpeed : walkingBobbingSpeed;
        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            //Player moving.
            timer += Time.deltaTime * bobbingSpeed;
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * 
                bobbingAmount, playerCamera.transform.localPosition.z);

            hand.transform.localPosition = new Vector3(defaultHandPosX + Mathf.Sin(timer) *
                bobbingAmount, hand.transform.localPosition.y, hand.transform.localPosition.z);

        }
        else
        {
            //Idle
            timer = 0;
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x,
                Mathf.Lerp(playerCamera.transform.localPosition.y, defaultPosY, Time.deltaTime * bobbingSpeed), 
                playerCamera.transform.localPosition.z);
            hand.transform.localPosition = new Vector3(Mathf.Lerp(hand.transform.localPosition.x, defaultHandPosX, Time.deltaTime * bobbingSpeed),
                hand.transform.localPosition.y,
                hand.transform.localPosition.z);
        }
    }

    void movement() {
        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        // Press Left Shift to run
        isRunning = runAction.IsPressed();
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * moveAction.ReadValue<Vector2>().y : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * moveAction.ReadValue<Vector2>().x : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
        #endregion
 
        #region Handles Gravity
        moveDirection.y = movementDirectionY;
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        #endregion
 
        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -lookAction.ReadValue<Vector2>().y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            hand.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookAction.ReadValue<Vector2>().x * lookSpeed, 0);
        }
 
        #endregion
    }
}