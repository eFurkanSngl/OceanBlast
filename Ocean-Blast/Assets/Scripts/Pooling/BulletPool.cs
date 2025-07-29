using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletSize;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private void Awake()
    {
        for(int i = 0; i < pool.Count; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
            pool.Enqueue(bullet);
        }
    }


    public GameObject GetBullet()
    {
        if(pool.Count > 0)
        {
            GameObject bullet = pool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.SetActive(false);
            return bullet;
        }
    }


    public void ReturnBullet (GameObject bullet)
    {
        pool.Enqueue(bullet);
        bullet.SetActive(false);
    }
}
