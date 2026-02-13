using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance; // Singleton for easy access
    public GameObject bulletPrefab;
    private GameObject singleBullet; // Only one bullet allowed

    void Awake() => Instance = this;

    void Start()
    {
        // Pre-instantiate the one bullet and hide it
        singleBullet = Instantiate(bulletPrefab);
        singleBullet.SetActive(false);
    }

    public GameObject GetBullet()
    {
        // Only return it if it's currently inactive (ready to fire)
        if (singleBullet != null && !singleBullet.activeInHierarchy)
        {
            return singleBullet;
        }
        return null; // Return null if bullet is already on screen
    }
}