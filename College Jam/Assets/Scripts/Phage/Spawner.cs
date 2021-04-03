using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public int numberToSpawn;
    public GameObject prefab;

    void Start() {
        for (int i = 0; i < numberToSpawn; i++) {
            GameObject obj = Instantiate(prefab, transform);
            obj.name = $"{prefab.name} {i}";
        }
    }
}
