using UnityEngine;

public class CupCollision : MonoBehaviour
{
    private AudioSource cupAudio;

    void Start()
    {
        
        cupAudio = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "Ball")
        {
            cupAudio.Play();
        }
    }
}
