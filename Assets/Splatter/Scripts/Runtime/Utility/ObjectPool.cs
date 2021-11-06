using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Splatter.Utility {
    public class ObjectPool : Singleton<ObjectPool> {
        private List<GameObject> pooledObjects;
        [SerializeField] private GameObject objectToPool;
        [SerializeField] private int initialNumberToPool;

        protected override void AwakeSingleton() {
            pooledObjects = new List<GameObject>();

            if (!objectToPool) {
                throw new InvalidOperationException("Select object to pool");
            }
        }

        private void Start() {
            for (int i = 0; i < initialNumberToPool; i++) {
                CreateItem();
            }
        }

        public GameObject GetObject(Vector3 position, Quaternion rotation, bool activateImmediately = true) {
            var item = GetOrCreateItem();

            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(activateImmediately);

            return item;
        }

        private GameObject GetOrCreateItem() {
            var item = pooledObjects.FirstOrDefault(i => !i.activeInHierarchy);

            return item ?? CreateItem();
        }

        private GameObject CreateItem() {
            var item = Instantiate(objectToPool);
            item.SetActive(false);
            pooledObjects.Add(item);

            return item;
        }
    }
}
