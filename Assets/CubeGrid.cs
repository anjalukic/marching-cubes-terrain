using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Cube
{
    public Vector3[] coords;
    public float[] values;

    //Constructor (not necessary, but helpful)
    public Cube(Vector3[] coords, float[] values)
    {
        this.coords = coords;
        this.values = values;
    }
}


public class CubeGrid : MonoBehaviour
{
    public int width = 100;
    int oldWidth = -1;
    public int height = 10;
    int oldHeight = -1;
    public float isolevel = 6.5f;
    private float oldIsolevel = -1f;
    public MarchingCube marchingCube;
    public Material cubeMat;
    public float cubeSize = 1.0f;
    private float oldCubeSize = -1.0f;
    public bool renderGrid = false;
    public float brushSize = 1.0f;
    public float brushIntensity = 0.5f;
    private List<Cube> cubes;

    void Update()
    {

        marchingCube = gameObject.GetComponentInChildren<MarchingCube>();
        if (cubeSize != oldCubeSize || isolevel != oldIsolevel || height != oldHeight || width != oldWidth)
        {
            oldCubeSize = cubeSize;
            oldIsolevel = isolevel;
            oldWidth = width;
            oldHeight = height;

            marchingCube.clearVertices();
            cubes = new List<Cube>();

            if (renderGrid)
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Cube");
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    Destroy(gameObjects[i]);
                }
            }

            generateGridAndRender();
            
            marchingCube.renderMesh();
        }

        if (Input.GetMouseButton(0))
        {
            marchingCube.clearVertices();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f)){
                foreach (Cube cube in cubes){
                    for (int i=0; i<8; i++){
                        if (Vector3.Distance(cube.coords[i], hit.point) < brushSize) {
                            if (cube.coords[i].y >= (height+1) * cubeSize)
                                cube.values[i] = isolevel;
                            else
                                cube.values[i] -= brushIntensity;
                        }
                    }
                    //render again
                    marchingCube.setValues(cube.coords, cube.values, isolevel);
                }
                marchingCube.renderMesh();
            }
        }
        if (Input.GetMouseButton(1))
        {
            marchingCube.clearVertices();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                foreach (Cube cube in cubes)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (Vector3.Distance(cube.coords[i], hit.point) < brushSize)
                        {
                            if (cube.coords[i].y <= 0f)
                                cube.values[i] = isolevel;
                            else
                            //if (cube.values[i] > isolevel){
                                cube.values[i] += brushIntensity;
                            //}
                        }
                    }
                    //render again
                    marchingCube.setValues(cube.coords, cube.values, isolevel);
                }
                marchingCube.renderMesh();
            }
        }

    }

    void generateGridAndRender()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float xCoord = x * cubeSize;
                    float yCoord = y * cubeSize;
                    float zCoord = z * cubeSize;

                    if (renderGrid)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(cubeSize / 2f + xCoord, cubeSize / 2f + yCoord, cubeSize / 2f + zCoord);
                        cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                        cube.tag = "Cube";
                        cube.GetComponent<MeshRenderer>().material = cubeMat;
                    }

                    Vector3[] coords = new Vector3[8];
                    float[] values = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        coords[i] = getVertexCoords(xCoord, yCoord, zCoord, i);
                        values[i] = generateDensity(coords[i]);
                    }
                    cubes.Add(new Cube(coords, values));
                    marchingCube.setValues(coords, values, isolevel);
                }
            }
        }
    }


    Vector3 getVertexCoords(float cubeX, float cubeY, float cubeZ, int i)
    {
        switch (i)
        {
            case 0:
                return new Vector3(0f + cubeX, 0f + cubeY, 0f + cubeZ);
            case 1:
                return new Vector3(0f + cubeX, 0f + cubeY, cubeSize + cubeZ);
            case 2:
                return new Vector3(cubeSize + cubeX, 0f + cubeY, cubeSize + cubeZ);
            case 3:
                return new Vector3(cubeSize + cubeX, 0f + cubeY, 0f + cubeZ);
            case 4:
                return new Vector3(0f + cubeX, cubeSize + cubeY, 0f + cubeZ);
            case 5:
                return new Vector3(0f + cubeX, cubeSize + cubeY, cubeSize + cubeZ);
            case 6:
                return new Vector3(cubeSize + cubeX, cubeSize + cubeY, cubeSize + cubeZ);
            case 7:
                return new Vector3(cubeSize + cubeX, cubeSize + cubeY, 0f + cubeZ);
        }
        return new Vector3();
    }

    float generateDensity(Vector3 vertexCoords)
    {
        // sphere
        //return Mathf.Pow(vertexCoords.x - (float)width/2*cubeSize,2) + Mathf.Pow(vertexCoords.y-(float)height/2 * cubeSize, 2) + Mathf.Pow(vertexCoords.z-(float)width/2 * cubeSize, 2);
        return vertexCoords.y + height * Mathf.PerlinNoise((float)vertexCoords.x / width * 8.03f, (float)vertexCoords.z / width * 8.03f);
    }
}
