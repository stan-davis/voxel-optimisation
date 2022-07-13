using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel
{
    public byte type;
}

public static class VoxelMeshData
{
    //ORDER OF VERTICES (TOP LEFT, TOP RIGHT, BOTTOM LEFT, BOTTOM RIGHT
    public static Vector3[][] vertices =
    {
        new Vector3[] { new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0)  }, //top +y
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)  }, //bottom -y
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0)  }, //front -z
        new Vector3[] { new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1)  }, //back +z
        new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0)  }, //right +x
        new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1)  }  //left -x
    };
}

public static class VoxelMeshDataRegular
{
    //ORDER OF VERTICES (TOP LEFT, TOP RIGHT, BOTTOM RIGHT, BOTTOM LEFT
    public static Vector3[][] vertices =
    {
        new Vector3[] { new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1) }, //top
        new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0) }, //bottom
        new Vector3[] { new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1) }, //front
        new Vector3[] { new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0) }, //back
        new Vector3[] { new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0) }, //right
        new Vector3[] { new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1) }  //left
    };
}

public enum Direction
{
    TOP,
    BOTTOM,
    FRONT,
    BACK,
    RIGHT,
    LEFT
}