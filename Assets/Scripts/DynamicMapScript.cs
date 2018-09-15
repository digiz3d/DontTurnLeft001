using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMapScript : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int blocksAhead = 2;
    [SerializeField] private GameObject[] blocksToSpawn;
    [SerializeField] private Transform mapContainerTransform;
    [SerializeField] private SwipeDetection player;

    private List<GameObject> spawnedBlocks;
    private float longuestBlock = 0f;

    // Use this for initialization
    void Start()
    {
        foreach (GameObject g in blocksToSpawn)
        {
            BlockScript b = g.GetComponent<BlockScript>();
            float size = b.GetEndPosition().z - b.GetStartPosition().z;

            longuestBlock = Mathf.Max(longuestBlock, size);
        }
        spawnedBlocks = new List<GameObject>();
        NewLevel();
    }

    void Update()
    {
        Vector3 movement = new Vector3(0, 0, -speed * Time.deltaTime);
        
        List<GameObject> blocksToDelete = new List<GameObject>();
        
        foreach (GameObject g in spawnedBlocks)
        {
            Transform t = g.transform;
            t.Translate(movement);

            if (t.localPosition.z < -longuestBlock)
            {
                blocksToDelete.Add(g);
            }
        }

        foreach (GameObject g in blocksToDelete)
        {
            spawnedBlocks.Remove(g);
            Destroy(g);
            SpawnRandomBlock();
        }
    }

    void SpawnRandomBlock()
    {
        int i = Random.Range(0, blocksToSpawn.Length);
        GameObject blockToSpawn = blocksToSpawn[i];
        BlockScript blockToSpawnScript = blockToSpawn.GetComponent<BlockScript>();

        GameObject lastBlock = spawnedBlocks[spawnedBlocks.Count - 1];
        BlockScript lastBlockScript = lastBlock.GetComponent<BlockScript>();

        GameObject spawnedBlock = Instantiate(blockToSpawn, lastBlock.transform.position + (lastBlockScript.GetEndPosition() - blockToSpawnScript.GetStartPosition()), Quaternion.identity, mapContainerTransform);
        spawnedBlocks.Add(spawnedBlock);
    }

    public void NewLevel()
    {
        if (spawnedBlocks.Count > 0)
        {
            for (int x = spawnedBlocks.Count - 1; x >= 0; x--)
            {
                GameObject g = spawnedBlocks[x];
                Destroy(g);
                spawnedBlocks.Remove(g);
            }
        }

        spawnedBlocks.Add(Instantiate(blocksToSpawn[0], Vector3.zero, Quaternion.identity, mapContainerTransform));

        while (blocksAhead > spawnedBlocks.Count)
        {
            SpawnRandomBlock();
        }
    }
}
