using UnityEngine;

public class PowerupBehaviour : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isActive", true);
    }
}
