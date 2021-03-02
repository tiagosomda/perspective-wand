using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementProvider : LocomotionProvider
{
    public float speed = 1.0f;
    public float jumpForce = 1.0f;
    public float jumpTime = 1.0f;
    public float gravityMultiplier = 1.0f;
    public List<XRController> controllers = null;
    public List<XRController> jumpControllers = null;

    private CharacterController characterController = null;
    private GameObject head = null;

    private bool jumpButtonPressed = false;
    private bool isJumping = false;
    private float jumpingCountdown;
    private Vector3 movement = Vector3.zero;

    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
    }

    private void Start()
    {
        PositionController();
    }

    private void Update()
    {
        PositionController();
        CheckForInput();
        ApplyVerticalMovement();
    }

    private void PositionController()
    {
        // get the head in local, playspace ground
        float headHeigth = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHeigth;

        // cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height/2;
        newCenter.y += characterController.skinWidth;
        
        // let's move the capsule in local space as well
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;
        
        // apply
        characterController.center = newCenter;
    }

    private void CheckForInput()
    {
        foreach(XRController controller in controllers)
        {
            if(controller.enableInputActions)
            {
                CheckForMovement(controller.inputDevice);
            }
        }

        bool isPressingJump = false;
        foreach (XRController controller in jumpControllers)
        {
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed))
            {
                if (pressed)
                {
                    isPressingJump = true;
                    break;
                }
            }
        }

        jumpButtonPressed = isPressingJump;
    }

    private void CheckForMovement(InputDevice device)
    {
        movement = Vector3.zero;
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        {
            StartMove(position);
        }
    }

    private void StartMove(Vector2 position)
    {
        // apply the touch position to the head's forward Vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        // rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        // apply speed and move
        movement += direction * speed;
    }

    private void ApplyVerticalMovement()
    {
        //if (isJumping == false && characterController.isGrounded && Input.GetKey(KeyCode.Space))
        if (isJumping == false && characterController.isGrounded && jumpButtonPressed)
        {
            isJumping = true;
            jumpingCountdown = jumpTime;
        }

        if (isJumping == false)
        {

            movement.y += (Physics.gravity.y * gravityMultiplier) * Time.deltaTime;
        }


        if (isJumping)
        {
            if (jumpingCountdown > 0)
            {
                jumpingCountdown -= Time.deltaTime;
                movement.y = jumpingCountdown * jumpForce;
            }
            else
            {
                isJumping = false;
            }

            if (jumpButtonPressed == false)
            {
                isJumping = false;
            }
        }

        characterController.Move(movement * Time.deltaTime);
    }
}
