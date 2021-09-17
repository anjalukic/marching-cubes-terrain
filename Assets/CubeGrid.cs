using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using
UnityEngine.UI;
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
    float width = 10f;
    float height = 3f;
    public float isolevel = 6.5f;
    private float oldIsolevel = -1f;
    public MarchingCube marchingCube;
    public Material cubeMat;
    public float cubeSize = 0.2f;
    private float oldCubeSize = -1.0f;
    public bool renderGrid = false;
    public float brushSize = 1.0f;
    public float brushIntensity = 0.5f;
    private List<Cube> cubes;
    public Text cubeSizeText;
    public Text brushSizeText;
    public Text brushIntensityText;


    private void Start()
    {
        marchingCube = gameObject.GetComponentInChildren<MarchingCube>();
    }
    void Update()
    {

        if (cubeSize != oldCubeSize || isolevel != oldIsolevel)
        {
            oldCubeSize = cubeSize;
            oldIsolevel = isolevel;
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
                            if (cube.coords[i].y < height)
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
                            if (cube.coords[i].y > 0f)
                                cube.values[i] += brushIntensity;
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
        int numOfCubesWidth = Mathf.RoundToInt(width/cubeSize);
        int numOfCubesHeight = Mathf.RoundToInt(height / cubeSize);

        for (int x = 0; x < numOfCubesWidth; x++)
        {
            for (int y = 0; y < numOfCubesHeight; y++)
            {
                for (int z = 0; z < numOfCubesWidth; z++)
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
        //return vertexCoords.y + height * Mathf.PerlinNoise((float)vertexCoords.x / width * 8.03f, (float)vertexCoords.z / width * 8.03f);
        return vertexCoords.y;
    }

    public void OnCubeSizeSliderChanged(float value)
    {
        cubeSize = value/4f;
        cubeSizeText.text = "Voxel size: " + cubeSize;
    }
    public void OnBrushSizeSliderChanged(float value)
    {
        brushSize = value;
        brushSizeText.text = "Brush size: " + brushSize.ToString("F2");
    }
    public void OnBrushIntensitySliderChanged(float value)
    {
        brushIntensity = value;
        brushIntensityText.text = "Brush intensity: " + brushIntensity.ToString("F2");
    }
}
