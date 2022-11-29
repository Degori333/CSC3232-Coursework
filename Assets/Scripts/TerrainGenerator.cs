using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public int rightSize = 20;
    public int leftSize = 20;
    public int frontSize = 20;
    public int backSize = 20;

    [SerializeField]
    private float terrainSize = 1f;

    [SerializeField]
    private float depression;
    [SerializeField]
    private float scale;
    [SerializeField][Min(1f)]
    private float[] frequency;
    [SerializeField][Min(0f)]
    private float[] ampiltude;
    private float offsetX;
    private float offsetZ;

    private float offsetRange = 4096;

    public int pixWidth = 256;
    public int pixHeight = 256;

    private Texture2D texture;
    private Renderer rend;
    [SerializeField]
    private Gradient gradient;

    private float minTerrainHeight;
    private float maxTerrainHeight;



    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(pixWidth, pixHeight);

        offsetX = Random.Range(-offsetRange, offsetRange);
        offsetZ = Random.Range(-offsetRange, offsetRange);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rend = GetComponent<Renderer>();
        rend.material.mainTexture = texture;

    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
        
    }


    void CreateShape()
    {
        vertices = new Vector3[(rightSize + leftSize + 1) * (frontSize + backSize + 1)];

        for (int i = 0 , z = -backSize; z <= frontSize; ++z)
        {         
            for (int x = -leftSize; x <= rightSize; ++x)
            {
                float y = 0f;
                for (int o = 0; o < frequency.Length; o++)
                {
                    y += ampiltude[o] * Mathf.PerlinNoise(x * frequency[o] + offsetX, z * frequency[o] + offsetZ) * scale;
                }
                vertices[i] = new Vector3(x * terrainSize, (y - depression) * terrainSize, z * terrainSize);
                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }
                ++i;
            }
        }

        triangles = new int[(leftSize + rightSize) * (frontSize + backSize) * 6];

        int vert = 0;
        int tris = 0;
        for (int z = -backSize; z<frontSize; ++z)
        {
            for (int x = -leftSize; x < rightSize; ++x)
            {

                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + leftSize + rightSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + leftSize + rightSize + 1;
                triangles[tris + 5] = vert + leftSize + rightSize + 2;

                ++vert;
                tris += 6;
            }
            ++vert;
        }

        colours = new Color[vertices.Length];
        for (int i = 0, z = -backSize; z <= frontSize; ++z)
        {
            for (int x = -leftSize; x <= rightSize; ++x)
            {
                float height = Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, vertices[i].y);
                colours[i] = gradient.Evaluate(height);
                //texture.SetPixel((int)vertices[i].x, (int)vertices[i].z, colours[i]);
                ++i;
            }
        }
        texture.Apply();
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.colors = colours;

        mesh.RecalculateNormals();
    }

    void SetCollider()
    {
        MeshCollider collider = GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }
}
