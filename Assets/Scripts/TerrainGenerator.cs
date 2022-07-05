using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject chunkPrefab;

    [SerializeField] private int worldSize = 1;
    [SerializeField] private int amplitude = 1;
    [SerializeField] private float scale = 1;
    [SerializeField] private int offset = 1;

    private void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        for (int x = -worldSize; x < worldSize; x++)
            for (int z = -worldSize; z < worldSize; z++)
            {
                CreateChunk(x, z);
            }
    }

    private void CreateChunk(int chunkX, int chunkZ)
    {
        Vector3 chunkPos = new Vector3(chunkX * Chunk.CHUNK_SIZE, 0, chunkZ * Chunk.CHUNK_SIZE);
        GameObject chunkInstance = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, transform);
        chunkInstance.name = "Chunk " + chunkX.ToString() + ", " + chunkZ.ToString();
        Chunk chunkObject = chunkInstance.GetComponent<Chunk>();

        chunkObject.data = GenerateTerrainData(chunkX, chunkZ);

        chunkObject.GenerateMesh();
    }

    private Voxel[,,] GenerateTerrainData(int chunkX, int chunkZ)
    {
        Voxel[,,] data = new Voxel[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];

        for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    data[x,y,z] = GenerateVoxelData(x + (chunkX * Chunk.CHUNK_SIZE), y, z + (chunkZ * Chunk.CHUNK_SIZE));
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
