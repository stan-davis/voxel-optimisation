using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct CulledChunkJob : IJob
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

        //top
        if (y == Chunk.CHUNK_SIZE - 1 || data[PositionToIndex(x, y + 1, z)].type == 0)
        {
            AppendQuad(position, Direction.TOP);
        }

        //bottom
        if (position.y == 0 || data[PositionToIndex(x, y - 1, z)].type == 0)
        {
            AppendQuad(position, Direction.BOTTOM);
        }

        //front
        if (position.z == Chunk.CHUNK_SIZE - 1 || data[PositionToIndex(x, y, z + 1)].type == 0)
        {
            AppendQuad(position, Direction.FRONT);
        }

        //back
        if (position.z == 0 || data[PositionToIndex(x, y, z - 1)].type == 0)
        {
            AppendQuad(position, Direction.BACK);
        }

        //right
        if (position.x == Chunk.CHUNK_SIZE - 1 || data[PositionToIndex(x + 1, y, z)].type == 0)
        {
            AppendQuad(position, Direction.RIGHT);
        }

        //left
        if (position.x == 0 || data[PositionToIndex(x - 1, y, z)].type == 0)
        {
            AppendQuad(position, Direction.LEFT);
        }
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
