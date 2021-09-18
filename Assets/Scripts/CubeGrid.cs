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
    float width = 15f;
    float height = 5f;
    float isolevel = 5f;
    private float oldIsolevel = -1f;
    public MarchingCube marchingCube;
    public Material cubeMat;
    float cubeSize = 0.2f;
    private float oldCubeSize = -1.0f;
    public bool renderGrid = false;
    float brushSize = 0.2f;
    float brushIntensity = 0.5f;
    private List<Cube> cubes;
    public Text cubeSizeText;
    public Text brushSizeText;
    public Text brushIntensityText;
    public Slider cubeSizeSlider;
    public Slider brushSizeSlider;
    public Slider brushIntensitySlider;
    public GameObject cursor;
    public float densityAmplitude = 0.5f;
    public float densityFrequency = 5f;
    float oldAmplitude = -1;
    float oldFrequency = -1;
    public InputField flattenInput;
    private bool flatten = false;
    private Material cursorMat;
    private Color white, red, green;


    private void Start()
    {
        marchingCube = gameObject.GetComponentInChildren<MarchingCube>();
        cursorMat = cursor.GetComponent<MeshRenderer>().material;
        white = new Color(1, 1, 1, 0.2f);
        red = new Color(1, 0, 0, 0.2f);
        green = new Color(0, 1, 0, 0.2f);

    }

 
    void Update()
    {
        //follow cursor
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            cursor.transform.position = hit.point;
            cursor.transform.localScale = new Vector3(brushSize*2, brushSize*2, brushSize * 2);
        }

        if (cubeSize != oldCubeSize || isolevel != oldIsolevel || densityAmplitude != oldAmplitude || densityFrequency != oldFrequency)
        {
            oldCubeSize = cubeSize;
            oldIsolevel = isolevel;
            oldFrequency = densityFrequency;
            oldAmplitude = densityAmplitude;
            

            if (renderGrid)
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Cube");
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    Destroy(gameObjects[i]);
                }
            }

            generateGridAndRender();
            
        }

        if (Input.GetMouseButton(0))
        {
            marchingCube.clearVertices();

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f)){
                cursorMat.SetColor("_Color", green);
                foreach (Cube cube in cubes){
                    for (int i=0; i<8; i++){
                        if (Vector3.Distance(cube.coords[i], hit.point) < brushSize) {
                            if (cube.coords[i].y < height)
                                cube.values[i] -= (brushSize - Vector3.Distance(cube.coords[i], hit.point)) * brushIntensity/(cubeSize*cubeSize)/10;
                        }
                    }
                    //render again
                    marchingCube.setValues(cube.coords, cube.values, isolevel);
                }
                marchingCube.renderMesh();
            }
        } else if (Input.GetMouseButton(1))
        {
            marchingCube.clearVertices();

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                cursorMat.SetColor("_Color", red);
                cursor.transform.position = hit.point;
                foreach (Cube cube in cubes)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (Vector3.Distance(cube.coords[i], hit.point) < brushSize)
                        {
                            if (cube.coords[i].y > 0f)
                                cube.values[i] += (brushSize - Vector3.Distance(cube.coords[i], hit.point)) * brushIntensity/(cubeSize*cubeSize)/10;
                        }
                    }
                    //render again
                    marchingCube.setValues(cube.coords, cube.values, isolevel);
                }
                marchingCube.renderMesh();
            }
        } else
        {
            cursorMat.SetColor("_Color", white);
        }

    }

    void generateGridAndRender()
    {
        marchingCube.clearVertices();
        cubes = new List<Cube>();
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

        marchingCube.renderMesh();
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
        if (flatten)
        {
            return vertexCoords.y;
        }
        float density = vertexCoords.y;
        density += height * Mathf.PerlinNoise(vertexCoords.x / width * densityFrequency, vertexCoords.z / width * densityFrequency) * densityAmplitude;
        return density;
    }

    public void OnCubeSizeSliderChanged(float value)
    {
        switch (value)
        {
            case 0: cubeSize = 0f;
                break;
            case 1: cubeSize = 0.2f;
                break;
            case 2:
                cubeSize = 0.5f;
                break;
            case 3:
                cubeSize = 1.0f;
                break;

        }
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

    public float getWidth()
    {
        return width;
    }
    public float getHeight()
    {
        return height;
    }

    public void flattenToValue(){
        float result;
        if (float.TryParse(flattenInput.text, out result)){
            flatten = true;
            isolevel = result;
            oldIsolevel = isolevel;
            generateGridAndRender();
            flatten = false;
        }
            
    }

    public void refreshTerrain()
    {
        isolevel = 5f;
        oldIsolevel = 5f;
        densityFrequency = Random.Range(1f, 6f);
        densityAmplitude = Random.Range(0.2f, 1.2f);
        generateGridAndRender();
    }
}
