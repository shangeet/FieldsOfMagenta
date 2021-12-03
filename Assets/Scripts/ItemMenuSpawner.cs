using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenuSpawner : MonoBehaviour {
    [SerializeField] public GameObject itemPrefabToSpawn;

    public GameObject SpawnPrefab() {
        print(itemPrefabToSpawn);
        return  (GameObject) Instantiate(itemPrefabToSpawn);
    }
}
