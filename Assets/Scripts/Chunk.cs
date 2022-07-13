using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public enum ChunkMethod { Lazy, Culled, Runs };

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 16;
    public const int CHUNK_SIZE_CUBED = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;

    public ChunkMethod method = ChunkMethod.Culled;

    public Voxel[] data = new Voxel[CHUNK_SIZE_CUBED];

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();

        switch(method)
        {
            case ChunkMethod.Lazy:
                LazyChunk(mesh);
                break;
            case ChunkMethod.Culled:
                CulledChunk(mesh);
                break;
            case ChunkMethod.Runs: 
                RunsChunk(mesh);
                break;
        }

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LazyChunk(Mesh mesh)
    {
        LazyChunkJob chunkJob = new LazyChunkJob();

        chunkJob.vertices = new NativeList<Vector3>(Allocator.TempJob);
        chunkJob.triangles = new NativeList<int>(Allocator.TempJob);
        chunkJob.data = new NativeArray<Voxel>(CHUNK_SIZE_CUBED, Allocator.TempJob);

        for (int i = 0; i < data.Length; i++)
        {
            chunkJob.data[i] = data[i];
        }

        JobHandle handle = chunkJob.Schedule();
        handle.Complete();

        mesh.SetVertices(chunkJob.vertices.ToArray());
        mesh.SetTriangles(chunkJob.triangles.ToArray(), 0);

        chunkJob.vertices.Dispose();
        chunkJob.triangles.Dispose();
        chunkJob.data.Dispose();
    }

    private void CulledChunk(Mesh mesh)
    {
        //Start Job
        CulledChunkJob chunkJob = new CulledChunkJob();

        chunkJob.vertices = new NativeList<Vector3>(Allocator.TempJob);
        chunkJob.triangles = new NativeList<int>(Allocator.TempJob);
        chunkJob.data = new NativeArray<Voxel>(CHUNK_SIZE_CUBED, Allocator.TempJob);

        for (int i = 0; i < data.Length; i++)
        {
            chunkJob.data[i] = data[i];
        }

        JobHandle handle = chunkJob.Schedule();
        handle.Complete();

        //Set vertices from job vertices, triangles, etc
        mesh.SetVertices(chunkJob.vertices.ToArray());
        mesh.SetTriangles(chunkJob.triangles.ToArray(), 0);

        //Dispose of job variables to avoid memory leaks
        chunkJob.vertices.Dispose();
        chunkJob.triangles.Dispose();
        chunkJob.data.Dispose();
    }

    private void RunsChunk(Mesh mesh)
    {
        GreedyChunkJob chunkJob = new GreedyChunkJob();

        chunkJob.vertices = new NativeList<Vector3>(Allocator.TempJob);
        chunkJob.triangles = new NativeList<int>(Allocator.TempJob);
        chunkJob.data = new NativeArray<Voxel>(CHUNK_SIZE_CUBED, Allocator.TempJob);

        chunkJob.visitedXN = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
        chunkJob.visitedXP = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
        chunkJob.visitedYN = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
        chunkJob.visitedYP = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
        chunkJob.visitedZN = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
        chunkJob.visitedZP = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);

        for (int i = 0; i < data.Length; i++)
        {
            chunkJob.data[i] = data[i];
        }

        JobHandle handle = chunkJob.Schedule();
        handle.Complete();

        mesh.SetVertices(chunkJob.vertices.ToArray());
        mesh.SetTriangles(chunkJob.triangles.ToArray(), 0);

        chunkJob.vertices.Dispose();
        chunkJob.triangles.Dispose();
        chunkJob.data.Dispose();

        chunkJob.visitedXN.Dispose();
        chunkJob.visitedXP.Dispose();
        chunkJob.visitedYN.Dispose();
        chunkJob.visitedYP.Dispose();
        chunkJob.visitedZN.Dispose();
        chunkJob.visitedZP.Dispose();
    }
}