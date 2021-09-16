using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCube : MonoBehaviour
{
    Vector3[] coords = new Vector3[8];
    float[] values = new float[8];
    int cubeIndex = 0;
    public float isolevel = 6.5f;
    List<Vector3> vertices = new List<Vector3>();
    public Material material;

    public void setValues(Vector3[] coords, float[] values, float isolevel)
    {
        this.coords = coords;
        this.values = values;
        this.isolevel = isolevel;
        generateVertices();
        //renderMesh();
    }

    void generateIndex()
    {
        cubeIndex = 0;
        for (int i=0; i< 8; i++)
        {
            if (values[i] < isolevel) cubeIndex |= (int)Mathf.Pow(2,i);
        }
    }

    Vector3 VertexInterp(float isolevel, Vector3 coord1, Vector3 coord2, float val1, float val2)
    {
        float mu;

        if (Mathf.Abs(isolevel - val1) < 0.00001)
            return coord1;
        if (Mathf.Abs(isolevel - val2) < 0.00001)
            return coord2;
        if (Mathf.Abs(val1 - val2) < 0.00001)
            return coord1;
        mu = (isolevel - val1) / (val2 - val1);
        
        float x = coord1.x + mu * (coord2.x - coord1.x);
        float y = coord1.y + mu * (coord2.y - coord1.y);
        float z = coord1.z + mu * (coord2.z - coord1.z);
        Vector3 p = new Vector3(x,y,z);
        return p;
    }

    void generateVertices()
    {
        Vector3[] intersectionPoints = new Vector3[12];
        generateIndex();
        
        int edges = Tables.edgeTable[cubeIndex];
        if ((edges & 1) == 1)
            intersectionPoints[0] =  VertexInterp(isolevel, coords[0], coords[1], values[0], values[1]);
        if ((edges & 2) == 2)
            intersectionPoints[1] = VertexInterp(isolevel, coords[1], coords[2], values[1], values[2]);
        if ((edges & 4) == 4)
            intersectionPoints[2] = VertexInterp(isolevel, coords[2], coords[3], values[2], values[3]);
        if ((edges & 8) == 8)
            intersectionPoints[3] = VertexInterp(isolevel, coords[3], coords[0], values[3], values[0]);
        if ((edges & 16) == 16)
            intersectionPoints[4] = VertexInterp(isolevel, coords[4], coords[5], values[4], values[5]);
        if ((edges & 32) == 32)
            intersectionPoints[5] = VertexInterp(isolevel, coords[5], coords[6], values[5], values[6]);
        if ((edges & 64) == 64)
            intersectionPoints[6] = VertexInterp(isolevel, coords[6], coords[7], values[6], values[7]);
        if ((edges & 128) == 128)
            intersectionPoints[7] = VertexInterp(isolevel, coords[7], coords[4], values[7], values[4]);
        if ((edges & 256) == 256)
            intersectionPoints[8] = VertexInterp(isolevel, coords[0], coords[4], values[0], values[4]);
        if ((edges & 512) == 512)
            intersectionPoints[9] = VertexInterp(isolevel, coords[1], coords[5], values[1], values[5]);
        if ((edges & 1024) == 1024)
            intersectionPoints[10] = VertexInterp(isolevel, coords[2], coords[6], values[2], values[6]);
        if ((edges & 2048) == 2048)
            intersectionPoints[11] = VertexInterp(isolevel, coords[3], coords[7], values[3], values[7]);
        
        for (int i = 0; Tables.triTable[cubeIndex, i] != -1; i += 3)
        {
            vertices.Add(intersectionPoints[Tables.triTable[cubeIndex, i + 2]]);
            vertices.Add(intersectionPoints[Tables.triTable[cubeIndex, i + 1]]);
            vertices.Add(intersectionPoints[Tables.triTable[cubeIndex, i]]);
        }

    }

    public void renderMesh()
    {
        GetComponent<MeshRenderer>().material = material;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = new Vector2[vertices.Count];
        int[] tri =  new int[vertices.Count];
        for (int i=0; i<vertices.Count; i++)
        {
            tri[i] = i;
        }
        mesh.triangles = tri;
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        //Debug.Log("broj trouglova " + (float)vertices.Count / 3);

    }

    public void clearVertices()
    {
        vertices = new List<Vector3>();
    }

}
