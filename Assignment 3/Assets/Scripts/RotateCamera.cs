using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField, Range(10f, 200f)] private float rotationSpeed;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
