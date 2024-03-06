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
    InputAction flashlightFocusAction;

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
    bool flashlightFocus = false;
    [SerializeField] GameObject flashlight;
    [SerializeField] Light flashlightLight;

    [Category("Head Bobbing")]
    [SerializeField] float walkingBobbingSpeed = 6f;
    [SerializeField] float runningBobbingSpeed = 10f;
    [SerializeField] float bobbingAmount = 0.05f;

    float defaultPosY = 0;
    float defaultHandPosX = 0;
    float timer = 0;
    float timerL = 0;

    // Focus lantern

    float innerSpotDefault = 30;
    float spotAngleDefault = 90;
    
    CharacterController characterController;
    void Start()
    {
        // Inputs.
        flashlightLight = flashlight.GetComponentInChildren<Light>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("movement");
        lookAction = playerInput.actions.FindAction("Look");
        flashlightAction = playerInput.actions.FindAction("toggleFlashlight");
        flashlightFocusAction = playerInput.actions.FindAction("focusFlashlight");
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
        toggleflashlightFocus();
        headBobbing();
    }

    void toggleFlashlight() {
        flashlightOn = flashlightAction.IsPressed() ? !flashlightOn : flashlightOn;

       flashlight.GetComponentInChildren<Light>().enabled = flashlightOn;
    }
    void toggleflashlightFocus()
    {


        if (flashlightFocusAction.IsPressed())
        {
            flashlightLight.intensity = 8;
            flashlightLight.range = 20;
            flashlightLight.innerSpotAngle += (float)(Mathf.Sin(timerL) * 10f);
            flashlightLight.spotAngle -= (float)(Mathf.Sin(timerL) * 10f);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60, Time.deltaTime * 5f);
            flashlightFocus = true;
            timerL += Time.deltaTime;
            timerL = Mathf.Clamp(timerL, 0, 3.1f);
        }
        else{
            flashlightLight.intensity = 4;
            flashlightLight.range = 16;
            flashlightLight.innerSpotAngle = Mathf.Lerp(flashlightLight.innerSpotAngle, innerSpotDefault, Time.deltaTime * 10f);
            flashlightLight.spotAngle = Mathf.Lerp(flashlightLight.spotAngle, spotAngleDefault, Time.deltaTime * 10f);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 75,Time.deltaTime * 5f);
            flashlightFocus = false;
            timerL = 0;
        }
        flashlightLight.innerSpotAngle = Mathf.Clamp(flashlightLight.innerSpotAngle, 30, 50);
        flashlightLight.spotAngle = Mathf.Clamp(flashlightLight.spotAngle, 50, 90);
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