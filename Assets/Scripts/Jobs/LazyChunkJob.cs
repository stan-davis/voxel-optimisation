using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct LazyChunkJob : IJob
{
    public NativeArray<Voxel> data;

    public NativeList<Vector3> vertices;
    public NativeList<int> triangles;

    public void Execute()
    {
        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    if (data[PositionToIndex(x, y, z)].type == 0)
                        continue;

                    CreateVoxel(x, y, z);
                }

        CreateTriangles();
    }

    private void CreateVoxel(int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);

        AppendQuad(position, Direction.TOP);
        AppendQuad(position, Direction.BOTTOM);
        AppendQuad(position, Direction.RIGHT);
        AppendQuad(position, Direction.LEFT);
        AppendQuad(position, Direction.FRONT);
        AppendQuad(position, Direction.BACK);
    }

    private int PositionToIndex(int x, int y, int z)
    {
        return x + Chunk.CHUNK_SIZE * (y + Chunk.CHUNK_SIZE * z);
    }

    private void AppendQuad(Vector3 position, Direction direction)
    {
        for (int i = 0; i < 4; i++)
            vertices.Add(position + VoxelMeshDataRegular.vertices[(int)direction][i]);
    }


    private void CreateTriangles()
    {
        for (int i = 0; i < vertices.Length; i += 4)
        {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
            triangles.Add(i);
            triangles.Add(i + 2);
            triangles.Add(i + 3);
        }
    }
}
