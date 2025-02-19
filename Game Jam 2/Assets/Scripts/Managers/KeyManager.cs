using UnityEngine;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    [Header("Key Spawning")]
    [Tooltip("Prefab of the key object.")]
    public GameObject keyPrefab;

    [Tooltip("Possible spawn points for keys.")]
    public List<Transform> keySpawnPoints = new List<Transform>();

    [Tooltip("How many keys to spawn in total.")]
    public int totalKeysToSpawn = 3;

    [Header("Runtime Info")]
    public int keysCollected = 0;

    private void Start()
    {
        SpawnKeys();
    }

    private void SpawnKeys()
    {
        if (keyPrefab == null)
        {
            Debug.LogError("KeyManager: keyPrefab not assigned.");
            return;
        }
        // Shuffle or randomly pick from spawn points
        List<Transform> spawnPool = new List<Transform>(keySpawnPoints);

        // Spawn a certain number of keys
        for (int i = 0; i < totalKeysToSpawn; i++)
        {
            if (spawnPool.Count == 0)
            {
                Debug.LogWarning("KeyManager: Not enough spawn points for all keys!");
                break;
            }
            int randomIndex = Random.Range(0, spawnPool.Count);
            Transform spawnLocation = spawnPool[randomIndex];

            Instantiate(keyPrefab, spawnLocation.position, spawnLocation.rotation);

            // Remove that spawn point so it can't spawn another key
            spawnPool.RemoveAt(randomIndex);
        }
    }

    // Called by KeyPickup script when the player collects a key
    public void OnKeyCollected()
    {
        keysCollected++;
        Debug.Log("Keys Collected: " + keysCollected);
    }
}