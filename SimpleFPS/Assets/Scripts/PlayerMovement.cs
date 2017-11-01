using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class PlayerMovement : MonoBehaviour {

    #region Public members

    [Header("Player Movement Settings")]
    public float movementMultiplier = 5.0f;
    public float runMultiplier = 10.0f;
    public float crouchMultiplier = 3.0f;
    public float jumpSpeed = 5.0f;
    public float mouseSensitivity = 5.0f;

    [Header("Other")]
    [Tooltip("The distance at which a Ray will be fired towards the ground.")]
    public float groundingDistance = 1.0f;

    [Header("Debugging")]
    public bool debugActive = true;

    #endregion

    #region Private members

    private CharacterController charController;
    private Camera FPScamera;
    private float rotUpDown = 0f;
    private float verticalVelocity = 0;
    private bool isGrounded = true;
    private float initialGroundingDistance;

    private static float MIN_X_ROTATION = -50f;
    private static float MAX_X_ROTATION = 50f;
    private static float CHARACTER_HEIGHT = 1.8f;
    private static float CHARACTER_CROUCHING_HEIGHT = 0.9f;

    #endregion

    // Use this for initialization
    void Start () {
        charController = GetComponent<CharacterController>();
        FPScamera = GetComponentInChildren<Camera>();

        initialGroundingDistance = groundingDistance;
    }

    // Update is called once per frame
    void Update() {
        //rotation
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, rotLeftRight, 0);

        rotUpDown -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotUpDown = Mathf.Clamp(rotUpDown, MIN_X_ROTATION, MAX_X_ROTATION);
        FPScamera.transform.localRotation = Quaternion.Euler(rotUpDown, 0, 0);

        //movement
        float forwardSpeed = Input.GetAxis("Vertical");
        float sideSpeed = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.LeftShift))    // we are RUNNING
        {
            forwardSpeed *= runMultiplier;
            sideSpeed *= runMultiplier;
        } else if (Input.GetKey(KeyCode.LeftControl))    // we are CROUCHING
        {
            charController.height = CHARACTER_CROUCHING_HEIGHT;
            forwardSpeed *= crouchMultiplier;
            sideSpeed *= crouchMultiplier;
            groundingDistance = (initialGroundingDistance / 2.0f) + 0.1f;     //adding 0.1f because of the strange levitation caused by the CharacterController
        }
        else                                        //we are WALKING
        {
            forwardSpeed *= movementMultiplier;
            sideSpeed *= movementMultiplier;
            if(charController.height != CHARACTER_HEIGHT)                    //we are back to being tall again
            {
                charController.height = CHARACTER_HEIGHT;
                groundingDistance = initialGroundingDistance;
            }
        }

        //JUMPING
        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = jumpSpeed;
            isGrounded = false;
        }

        Vector3 movement = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);
        movement = transform.rotation * movement;

        charController.Move(movement * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundingDistance))
            isGrounded = true;

        if(debugActive)
            Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundingDistance), Color.blue);
    }
}
