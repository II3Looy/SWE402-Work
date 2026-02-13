using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 50.0f;

    bool moveTowardTop = false; 

    void Update()
    {
        // Vector3.forward is the standard "forward" direction in 3D space
        Vector3 direction = moveTowardTop ? Vector3.back : Vector3.forward;
        
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }
}
