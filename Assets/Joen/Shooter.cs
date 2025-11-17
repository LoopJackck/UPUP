using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shooter : MonoBehaviour
{
    public static Shooter poolinstance;

    public GameObject bulletPrefab;
    public int BulletCount = 10;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float ShootDelayTime = 3;

    private readonly Queue<GameObject> bulletQueue = new Queue<GameObject>();

    private void Awake()
    {
        if (poolinstance == null)
        {
            poolinstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < BulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletQueue.Enqueue(bullet);
        }
    }

    private void Start()
    {
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(ShootDelayTime); 
        }
    }

    public void Fire()
    {
        GameObject bullet = bulletQueue.Dequeue();
        bullet.SetActive(true);

        bullet.transform.position = firePoint.position;
        bullet.transform.right = firePoint.right; 

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.right * bulletSpeed;

        bulletQueue.Enqueue(bullet);
    }
}
