using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject chunkPrefab;

    [SerializeField] private int amplitude = 1;
    [SerializeField] private float scale = 1;
    [SerializeField] private int offset = 1;

    [SerializeField][Range(1, 10)] private int renderDistance;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ChunkMethod chunkMethod;

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

    private void Start()
    {
        loadedChunks.Clear();
    }

    private void Update()
    {
        RenderChunks();
    }

    private void RenderChunks()
    {
        int currentChunkX = Mathf.RoundToInt(playerTransform.position.x / Chunk.CHUNK_SIZE);
        int currentChunkZ = Mathf.RoundToInt(playerTransform.position.z / Chunk.CHUNK_SIZE);

        //Loop through all chunk coordinates within the render distance
        for (int x = currentChunkX - renderDistance; x <= currentChunkX + renderDistance; x++)
            for (int z = currentChunkZ - renderDistance; z <= currentChunkZ + renderDistance; z++)
            {
                Vector2Int currentIteration = new Vector2Int(x, z);

                if (!loadedChunks.TryGetValue(currentIteration, out Chunk chunk))
                    CreateChunk(x, z);
                else
                    chunk.gameObject.SetActive(true);
            }
        
        foreach(KeyValuePair<Vector2Int, Chunk> c in loadedChunks)
        {
            Vector2Int pos = c.Key;

            if (Mathf.Abs(currentChunkX - pos.x) > renderDistance || Mathf.Abs(currentChunkZ - pos.y) > renderDistance)
            {
                loadedChunks[pos].gameObject.SetActive(false);
            }
        }
    }

    private void CreateChunk(int chunkX, int chunkZ)
    {
        Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);

        Vector3 chunkWorldPos = new Vector3(chunkX * Chunk.CHUNK_SIZE, 0, chunkZ * Chunk.CHUNK_SIZE);
        GameObject chunkInstance = Instantiate(chunkPrefab, chunkWorldPos, Quaternion.identity, transform);
        chunkInstance.name = "Chunk " + chunkX.ToString() + ", " + chunkZ.ToString();
        Chunk chunkObject = chunkInstance.GetComponent<Chunk>();

        loadedChunks.Add(chunkPos, chunkObject);

        chunkObject.method = chunkMethod;
        chunkObject.data = GenerateTerrainData(chunkX, chunkZ);
        chunkObject.GenerateMesh();
    }

    private Voxel[] GenerateTerrainData(int chunkX, int chunkZ)
    {
        Voxel[] data = new Voxel[Chunk.CHUNK_SIZE_CUBED];

        for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    data[x + Chunk.CHUNK_SIZE * (y + Chunk.CHUNK_SIZE * z)] = GenerateVoxelData(x + (chunkX * Chunk.CHUNK_SIZE), y, z + (chunkZ * Chunk.CHUNK_SIZE));
                }

        return data;
    }

    private Voxel GenerateVoxelData(int x, int y, int z)
    {
        Voxel voxel = new Voxel();
        voxel.type = 0;

        float terrainHeight = Mathf.RoundToInt(Mathf.PerlinNoise((x + offset) / scale, (z + offset) / scale) * amplitude + 0.001f);

        if (y <= terrainHeight)
            voxel.type = 1;

        return voxel;
    }
}
