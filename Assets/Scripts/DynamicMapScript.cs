using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMapScript : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int blocksAhead = 2;
    [SerializeField] private GameObject[] blocksToSpawn;
    [SerializeField] private SwipeDetection player;
    [SerializeField] private Transform currentTrackPositionMarker;

    [Header("Debug: Some dynamic stuff")]
    private List<GameObject> spawnedBlocks;
    private List<GameObject> blocksToDelete;

    private float longuestBlock = 0f;
    private float currentAngle = 0;

    private GameObject currentBlock;
    private BlockScript currentBlockScript;
    private Vector3[] currentBlockTracksPoints;
    private Vector3 lastTrackPosition;

    private bool newBlock = false;
    // Use this for initialization
    void Start()
    {
        spawnedBlocks = new List<GameObject>();
        blocksToDelete = new List<GameObject>();

        foreach (GameObject g in blocksToSpawn)
        {
            BlockScript b = g.GetComponent<BlockScript>();
            float size = b.GetEndPosition().z - b.GetStartPosition().z;

            longuestBlock = Mathf.Max(longuestBlock, size);
        }
        NewLevel();
    }

    void Update()
    {
        Vector3 movement = new Vector3(0, 0, -speed * Time.deltaTime);
        movement = Quaternion.AngleAxis(currentAngle, Vector3.up) * movement;
        blocksToDelete.Clear();
        foreach (GameObject block in spawnedBlocks)
        {
            Transform blockTransform = block.transform;
            blockTransform.Translate(movement);
            BlockScript blockScript = block.GetComponent<BlockScript>();

            Vector3 endPosition = blockScript.GetEndPosition();
            newBlock = false;
            if (blockTransform.localPosition.z < player.gameObject.transform.position.z && player.gameObject.transform.position.z < (blockTransform.localPosition.z + endPosition.z))
            {
                if (currentBlock != block)
                {
                    currentBlock = block;
                    currentBlockScript = blockScript;
                    newBlock = true;
                    //Debug.Log("new block :)");
                }
            }

            if (blockTransform.localPosition.z < player.gameObject.transform.position.z - longuestBlock * 2)
            {
                blocksToDelete.Add(block);
            }
        }

        foreach (GameObject g in blocksToDelete)
        {
            DeleteBlock(g);
            SpawnRandomBlock();
        }

        if (currentBlockScript != null)
        {
            currentBlockTracksPoints = currentBlockScript.GetTracksPositions();

            if (newBlock)
            {
                lastTrackPosition = Vector3.zero;
            }

            /*/
            if (currentBlockTracksPoints.Length > 0)
            {
                Debug.Log("test" + currentBlockTracksPoints[0]);
                currentTrackPositionMarker.position = currentBlock.transform.localPosition + currentBlockTracksPoints[0] + new Vector3(0,2,0);
            }
            //*/
            Vector3 selectedTrackPoint = Vector3.zero;
            foreach (Vector3 trackPoint in currentBlockTracksPoints)
            {
                Vector3 realPointCoordinates = new Vector3(trackPoint.x, 0, trackPoint.z);
                if ((currentBlock.transform.localPosition.z + realPointCoordinates.z) < 0f)
                {
                    selectedTrackPoint = trackPoint;
                    Debug.Log(trackPoint.ToString());
                }
            }
            RotateMap(selectedTrackPoint.y);
            currentAngle = selectedTrackPoint.y;
            Vector3 realPointCoordinates2 = new Vector3(selectedTrackPoint.x, 0, selectedTrackPoint.z);
            currentTrackPositionMarker.position = currentBlock.transform.localPosition + realPointCoordinates2 + new Vector3(0, 2, 0);
        }
    }

    void SpawnRandomBlock()
    {
        int i = Random.Range(0, blocksToSpawn.Length);
        GameObject blockToSpawn = blocksToSpawn[i];
        BlockScript blockToSpawnScript = blockToSpawn.GetComponent<BlockScript>();

        GameObject lastBlock = spawnedBlocks[spawnedBlocks.Count - 1];
        BlockScript lastBlockScript = lastBlock.GetComponent<BlockScript>();

        GameObject spawnedBlock = Instantiate(blockToSpawn, lastBlock.transform.localPosition + (lastBlockScript.GetEndPosition() - blockToSpawnScript.GetStartPosition()), transform.rotation, transform);
        spawnedBlocks.Add(spawnedBlock);
    }

    void DeleteBlock(GameObject g)
    {
        spawnedBlocks.Remove(g);
        Destroy(g);
    }

    public void NewLevel()
    {
        if (spawnedBlocks.Count > 0)
        {
            for (int x = spawnedBlocks.Count - 1; x >= 0; x--)
            {
                GameObject g = spawnedBlocks[x];
                DeleteBlock(g);
            }
        }

        spawnedBlocks.Add(Instantiate(blocksToSpawn[0], Vector3.zero, Quaternion.identity, transform));

        while (blocksAhead > spawnedBlocks.Count)
        {
            SpawnRandomBlock();
        }
    }

    void RotateMap(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
    }
}
