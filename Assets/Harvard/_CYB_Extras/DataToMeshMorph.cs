using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;
using Mesh = UnityEngine.Mesh;

public class DataToMeshMorph : MonoBehaviour
{
    // Based on example from https://catlikecoding.com/unity/tutorials/procedural-grid/

    private Mesh mesh;
    private Vector3[] vertices;
    public int xSize, ySize;

    [Range(-20,100)]
    public float param1;
    [Range(-20, 100)]
    public float param2;
    [Range(-20, 100)]
    public float param3;
    [Range(-20, 100)]
    public float param4;

    public AtomicDataSwitch dataSwitch1;
    public AtomicDataSwitch dataSwitch2;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    float lateUpdateTime;

    // Update is called once per frame
    void Update()
    {
        if (Time.time < lateUpdateTime) 
            return;
        lateUpdateTime = Time.time + 0.1f;

        Animate();
    }

    private void Awake()
    {
        Generate();
    }

    private void Animate()
    {
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 
                    y + param1 * dataSwitch1.Value / 1024f * Mathf.Cos(i / param3), 
                    param2 * dataSwitch1.Value / 1024f * Mathf.Sin(i / param4));
            }
        }
        mesh.vertices = vertices;

        this.GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, dataSwitch2.Value / 1024f);

    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
            }
        }
        mesh.vertices = vertices;

        int[] triangles = new int[xSize * ySize * 6 * 2];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;

                //. do the back
                triangles[ti * 2 + 0] = triangles[ti + 0];
                triangles[ti * 2 + 1] = triangles[ti + 2];
                triangles[ti * 2 + 2] = triangles[ti + 1];

                triangles[ti * 2 + 3] = triangles[ti + 3];
                triangles[ti * 2 + 4] = triangles[ti + 5];
                triangles[ti * 2 + 5] = triangles[ti + 4];
            }
        }
        mesh.triangles = triangles;

        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.uv = uv;
        mesh.tangents = tangents;
    }
}
