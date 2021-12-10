using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenuSpawner : MonoBehaviour {
    [SerializeField] public GameObject itemPrefabToSpawn;

    public GameObject SpawnPrefab() {
        return  (GameObject) Instantiate(itemPrefabToSpawn);
    }
}
