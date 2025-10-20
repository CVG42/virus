using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<GameObject> pool = new Queue<GameObject>();

        private void Awake()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform);
                bullet.SetActive(false);
                pool.Enqueue(bullet);
            }
        }

        public GameObject GetBullet()
        {
            if (pool.Count == 0)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform);
                bullet.SetActive(false);
                pool.Enqueue(bullet);
            }

            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
    }
}
