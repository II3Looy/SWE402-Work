using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    private Rigidbody rb;
    private float moveInput;
    private float rotateInput;
    public float boundary = 48.0f; // Boundary limit for X and Z axes

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            // Standard: W/Up is 1, S/Down is -1
            float forward = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) ? 1f : 0f;
            float backward = (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) ? -1f : 0f;
            moveInput = forward + backward;

            // Standard: D/Right is 1, A/Left is -1
            float right = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) ? 1f : 0f;
            float left = (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) ? -1f : 0f;
            rotateInput = right + left;
        }
    }

void FixedUpdate()
    {
        // Move forward/backward based on where the player is facing
        Vector3 movement = transform.forward * moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate left/right
        float turn = rotateInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}