using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Jam.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public GameRules gameRules;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction flashlightAction;
    InputAction runAction;
    InputAction flashlightFocusAction;

    [Header("Movement")]
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject hand;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float gravity = 10f;

    [SerializeField] public float lookSpeed = 1f;
    [SerializeField] float lookXLimit = 80f;
    
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    bool canMove = true;
    bool isRunning = false;

    bool flashlightOn = true;
    bool flashlightFocus = false;
    [SerializeField] GameObject flashlight;
    [SerializeField] Light flashlightLight;

    [Header("Head Bobbing")]
    [SerializeField] float walkingBobbingSpeed = 6f;
    [SerializeField] float runningBobbingSpeed = 10f;
    [SerializeField] float bobbingAmount = 0.05f;

    float defaultPosY = 0;
    float defaultHandPosX = 0;
    float timer = 0;
    float timerL = 0;

    // Focus lantern
    [Header("Lantern Focus")]
    public float focusTimeLimit = 8f;
    public float focusTimerMultiplier = 1f;
    public float focusTimerMultiplierCooldown = 0.4f;
    float focusBarAmmount;
    float focusTime;
    float focusCoolDownTime = 8f;
    bool focusCoolDown = false;

    float innerSpotDefault = 30;
    float spotAngleDefault = 90;
    
    CharacterController characterController;

    [Header("Enemy Interaction")]
    public LayerMask isEnemy;


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

        defaultPosY = playerCamera.transform.localPosition.y;
        defaultHandPosX = hand.transform.localPosition.x;

        focusTime = focusTimeLimit;

        flashlightAction.performed += toggleFlashlight;
        EnemyAttackEvent.EnemyAttack += setAttacked;
    }
 
    void Update()
    {
        if (!gameRules.isPause)
        {
            if (canMove)
            {
                movement();
                toggleflashlightFocus();
                headBobbing();
            }
        }
    }

    void setAttacked(Transform enemy)
    {
        this.transform.LookAt(enemy);
        canMove = false;
    }

    void toggleFlashlight(InputAction.CallbackContext context) {
        //flashlightOn = flashlightAction.IsPressed() ? !flashlightOn : flashlightOn;
        if (gameRules.isPause)
            return;
        if(context.performed)
        {
            flashlightOn = !flashlightOn;
        }

        flashlight.GetComponentInChildren<Light>().enabled = flashlightOn;
    }

    void focusEnemy()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if(Physics.Raycast(ray, 15f, isEnemy))
        {
            EnemyFocusEvent.InvokeEnemyFocus(Time.deltaTime);
        }
        else
        {
            EnemyFocusEvent.InvokeEnemyExitFocus();
        }
    }
    void toggleflashlightFocus()
    {
        float delta = Time.deltaTime;

        if(focusTime <= 0)
        {
            focusCoolDown = true;
        }

        if(!(focusCoolDown && focusTime < focusCoolDownTime))
        {
            focusCoolDown = false;
        }

        if (flashlightFocusAction.IsPressed() && (!focusCoolDown && flashlightOn) )
        {
            flashlightLight.intensity = 8;
            flashlightLight.range = 20;
            flashlightLight.innerSpotAngle += (float)(Mathf.Sin(timerL) * 10f);
            flashlightLight.spotAngle -= (float)(Mathf.Sin(timerL) * 10f);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 50, delta * 5f);
            flashlightFocus = true;
            timerL += delta;
            focusTime -= delta * focusTimerMultiplier;
            timerL = Mathf.Clamp(timerL, 0, 3.1f);
            focusEnemy();
        }
        else{
            flashlightLight.intensity = 4;
            flashlightLight.range = 16;
            flashlightLight.innerSpotAngle = Mathf.Lerp(flashlightLight.innerSpotAngle, innerSpotDefault, delta * 10f);
            flashlightLight.spotAngle = Mathf.Lerp(flashlightLight.spotAngle, spotAngleDefault, delta * 10f);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 75, delta * 5f);
            flashlightFocus = false;
            timerL = 0;
            focusTime += delta * (focusCoolDown ? focusTimerMultiplierCooldown : focusTimerMultiplier);
            EnemyFocusEvent.InvokeEnemyExitFocus();
        }
        flashlightLight.innerSpotAngle = Mathf.Clamp(flashlightLight.innerSpotAngle, 30, 40);
        flashlightLight.spotAngle = Mathf.Clamp(flashlightLight.spotAngle, 40, 90);
        focusTime = Mathf.Clamp(focusTime, 0, focusTimeLimit);
        focusBarAmmount = focusTime / focusTimeLimit;
        StaminaChangeEvent.InvokeStaminaChange(focusBarAmmount, focusCoolDown);
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
        float curSpeedX = canMove ? (isRunning && !flashlightFocus ? runSpeed : walkSpeed) * moveAction.ReadValue<Vector2>().y : 0;
        float curSpeedY = canMove ? (isRunning && !flashlightFocus ? runSpeed : walkSpeed) * moveAction.ReadValue<Vector2>().x : 0;
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
        PlayerSoundEvent.InvokeFootStepEvent(Mathf.Abs(curSpeedX) + Mathf.Abs(curSpeedY), isRunning);
    }
}