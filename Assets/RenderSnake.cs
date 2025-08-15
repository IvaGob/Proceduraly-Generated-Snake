using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSnake : MonoBehaviour
{
    public Material material;
    [SerializeField]
    private float face_angle;
    [SerializeField]
    private float eyes_angle;
    [SerializeField]
    private float eyes_distance;
    [SerializeField]
    private GameObject eye_prefab;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uv = new List<Vector2>();
    private List<int> triangles = new List<int>(); 
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        //vertices.Clear();
        //vertices.Add(new Vector3(0, 1));
        //vertices.Add(new Vector3(1, 0));
        //vertices.Add(new Vector3(-1, 0));

        //uv.Clear();
        //uv.Add(new Vector2(0, 1));
        //uv.Add(new Vector2(1, 0));
        //uv.Add(new Vector2(-1, 0));

        //triangles.Clear();
        //triangles.Add(0);
        //triangles.Add(1);
        //triangles.Add(2);
        mesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
    }
    public void DrawMeshBetweenTwoSegments(Segment a, Segment b, int i)
    {
        //
        Vector2[] lr_global = a.GetLeftAndRightPointsOnSphere();
        Vector2[] lr_a = {ConvertGlobalToLocalPoint(lr_global[0]), ConvertGlobalToLocalPoint(lr_global[1])};
        lr_global = b.GetLeftAndRightPointsOnSphere();
        Vector2[] lr_b = { ConvertGlobalToLocalPoint(lr_global[0]), ConvertGlobalToLocalPoint(lr_global[1])};
        //
        int baseIndex = vertices.Count;
        vertices.Add(lr_a[0]);
        vertices.Add(lr_a[1]);
        vertices.Add(lr_b[0]);
        vertices.Add(lr_b[1]);
        //
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        //
        //int baseIndex = i * 4;
        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 2);

        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 3);

    }
    public void DrawMeshForTail(Segment a, int i)
    {
        //
        Vector2[] lr_global = a.GetLeftAndRightPointsOnSphere();
        Vector2[] lr_a = { ConvertGlobalToLocalPoint(lr_global[0]), ConvertGlobalToLocalPoint(lr_global[1]) };
        Vector2 end_p = ConvertGlobalToLocalPoint(a.GetBackPoint());
        //
        int baseIndex = vertices.Count;
        vertices.Add(lr_a[0]);
        vertices.Add(lr_a[1]);
        vertices.Add(end_p);
        //
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0.5f, 0));
        //
        //
        //int baseIndex = i * 4;
        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 2);
    }
    public void DrawMeshForHead(Segment a, int i)
    {
        //
        Vector2[] lr_global = a.GetLeftAndRightPointsOnSphere();
        Vector2[] lr_a = { ConvertGlobalToLocalPoint(lr_global[0]), ConvertGlobalToLocalPoint(lr_global[1]) };
        Vector2 start_p = ConvertGlobalToLocalPoint(a.GetPointOnCircleAtAngle(0));
        Vector2[] sides_p = { ConvertGlobalToLocalPoint(a.GetPointOnCircleAtAngle(face_angle)), ConvertGlobalToLocalPoint(a.GetPointOnCircleAtAngle(-face_angle))};
        //
        int baseIndex = vertices.Count;
        vertices.Add(lr_a[0]);
        vertices.Add(sides_p[0]);
        vertices.Add(sides_p[1]);
        vertices.Add(lr_a[1]);
        vertices.Add(start_p);
        //
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(0, 0.5f));
        uv.Add(new Vector2(1, 0.5f));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(0.5f, 1));
        //
        //
        //int baseIndex = i * 4 + 3;
        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 2);
        //
        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex + 3);
        //
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 4);
        triangles.Add(baseIndex + 2);
    }
    public void DrawSnakeMesh(Segment[] segments)
    {
        if (segments.Length < 2)
            return; // Занадто мало сегментів для побудови
        mesh.Clear();
        //
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
        //
        for (int i = 0; i < segments.Length-1; i++)
        {
            DrawMeshBetweenTwoSegments(segments[i], segments[i + 1],i);
            Debug.Log(i);   
        }
        //
        int index = segments.Length - 1;
        DrawMeshForTail(segments[index], index);
        DrawMeshForHead(segments[0], index);
        //
        DrawEyes(segments[0]);
        //
        if (uv.Count != vertices.Count)
        {
            Debug.LogError("UV count != vertices count. UV: " + uv.Count + " Vertices: " + vertices.Count);
        }
        if (triangles.Count % 3 != 0)
        {
            Debug.LogError("Triangles count is not divisible by 3: " + triangles.Count);
        }
        //
        gameObject.GetComponent<MeshFilter>().mesh.vertices = vertices.ToArray();
        gameObject.GetComponent<MeshFilter>().mesh.uv = uv.ToArray();
        gameObject.GetComponent<MeshFilter>().mesh.triangles = triangles.ToArray();
        gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }
    public void DrawEyes(Segment a)
    {
        Vector2[] eyes_positions = { a.GetPointOnCircleAtAngle(eyes_angle, eyes_distance), a.GetPointOnCircleAtAngle(-eyes_angle, eyes_distance) };
        if (!GameObject.FindGameObjectWithTag("Eye"))
        {
            Instantiate(eye_prefab, eyes_positions[0], Quaternion.identity);
            Instantiate(eye_prefab, eyes_positions[1], Quaternion.identity);
        }
        else
        {
            GameObject[] eyes = GameObject.FindGameObjectsWithTag("Eye");
            eyes[0].transform.position = eyes_positions[0];
            eyes[1].transform.position = eyes_positions[1];
        }
    }
    public Vector2 ConvertGlobalToLocalPoint(Vector2 point)
    {
        return transform.InverseTransformPoint(point);
    }
}
