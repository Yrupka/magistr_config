using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph_3d : MonoBehaviour
{
    private Mesh mesh;
    private Color[] colors;
    private Transform rotator; // объект, который обеспечивает вращение графика

    private List<float> y_normalize;
    private int x_count, z_count;
    private float min, max;

    private int y_points; // количество точек по у (высота)
    private float height; // допустимая высота графика
    private float graph_offset; // расстояние подписей от графика
    private float line_offset; // расстояние линии от графика

    Vector3[] vertices;

    public Gradient gradient;
    public GameObject text;
    public GameObject line;
    public Camera cam;

    void Start()
    {
        rotator = transform.parent;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;  

        List<float> x = new List<float>();
        List<float> random = new List<float>();
        List<float> z = new List<float>();
        for (int i = 0; i < 10; i++)
        {
            random.Add(Random.Range(0f, 40f));
        }
        x.AddRange(new List<float>(){1000, 1000, 1400, 2000, 2000, 2000, 5000, 5500});
        z.AddRange(new List<float>(){10, 11, 22, 5, 33, 13, 44, 26});

        Create_graph(x, random, z);
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) // правая кнопка мыши
        {
            rotator.Rotate(Vector3.up, Input.GetAxis("Mouse X"));

            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y == 1)
                    cam.transform.localPosition += new Vector3(1, 0, 1);
                if (Input.mouseScrollDelta.y == -1)
                    cam.transform.localPosition -= new Vector3(1, 0, 1);

            }
        }
    }

    public void Create_graph(List<float> x, List<float> y, List<float> z)
    {
        max = Mathf.Max(y.ToArray());
        min = Mathf.Min(y.ToArray());

        y_normalize = new List<float>();

        y_points = 4;
        height = 3f;
        graph_offset = 0.5f;
        line_offset = 0.4f;

        foreach (float item in y)
            y_normalize.Add(item / max * height);

        Clear_objects();
        Create_graph_object(x, z);

        x = x.Distinct().ToList(); // удаление дубликатов
        z = z.Distinct().ToList();
        x_count = x.Count;
        z_count = z.Count;

        Set_position();
        Create_labels(x, z);
        Create_lines();
    }

    private void Clear_objects() // удаляет все объекты предыдущего графика
    {
        for (int i = 1; i < transform.parent.childCount; i++)
            Destroy(transform.parent.GetChild(i).gameObject);
    }

    // опускает график, так чтобы минимальная точка лежала на высоте 0
    // настраевает камеру, для корректного отображения
    private void Set_position()
    {
        float fake_min = Mathf.Min(y_normalize.ToArray());
        transform.localPosition = new Vector3(-x_count / 2, -fake_min, -z_count / 2);
        cam.transform.localPosition = new Vector3(-x_count / 2 - 2, height / 2, -z_count / 2 - 2);
        cam.transform.rotation = Quaternion.Euler(new Vector3(15f, 45f, 0f));
    }

    // создает подписи осей
    private void Create_labels(List<float> x_data, List<float> z_data)
    {
        // x
        for (int i = 0; i < x_data.Count; i++)
        {
            GameObject instanse = Instantiate(text, rotator);
            instanse.transform.localPosition = new Vector3(-x_data.Count / 2 + i, 0, -z_data.Count / 2 - graph_offset);
            instanse.GetComponent<TextMesh>().text = x_data[i].ToString();
        }

        //z
        z_data.Sort();
        for (int i = 0; i < z_data.Count; i++)
        {
            GameObject instanse = Instantiate(text, rotator);
            instanse.transform.Rotate(Vector3.up, 90);
            instanse.transform.localPosition = new Vector3(-x_data.Count / 2 - graph_offset, 0, -z_data.Count / 2 + i);
            instanse.GetComponent<TextMesh>().text = z_data[i].ToString();
        }

        //y
        float step = height / y_points;
        float num_step = (max - min) / y_points;
        for (int i = 0; i < y_points; i++)
        {
            GameObject instanse = Instantiate(text, rotator);
            instanse.transform.Rotate(Vector3.up, 45);
            float y_offset = Mathf.Sqrt(Mathf.Pow(graph_offset, 2) * 2f) / 1.5f; // отступ от графика на расстояние гипотенузы треугольника со сторонами отступа
            Vector3 position = new Vector3(-x_data.Count / 2 - y_offset, 0, -z_data.Count / 2 - y_offset);

            if (i == 0)
            {
                position.y = 0;
                instanse.transform.localPosition = position;
                instanse.GetComponent<TextMesh>().text = min.ToString("#.00");
                continue;
            }
            if (i == y_points - 1)
            {
                position.y = height - Mathf.Min(y_normalize.ToArray());
                instanse.transform.localPosition = position;
                instanse.GetComponent<TextMesh>().text = max.ToString("#.00");
                continue;
            }

            position.y = step * i;
            instanse.transform.localPosition = position;
            instanse.GetComponent<TextMesh>().text = (num_step * i).ToString("#.00");
        }
    }

    //создает линии для осей
    private void Create_lines()
    {
        float arrow_proc = 0.25f; // процент, который будет занимать наконечник стелы от длины линии | наконечник стрелки занимает 1/4 от 1
        float line_indent = 0.5f; // отступ линии от последней точки значений
        for (int i = 0; i < 3; i++)
        {
            GameObject instanse = Instantiate(line, rotator);
            LineRenderer renderer = instanse.GetComponent<LineRenderer>();
            renderer.positionCount = 4; // количество отрезков в линии
            float arrow_offset = 0f; // 
            Vector3 ArrowOrigin = new Vector3(-x_count / 2 - line_offset, 0, -z_count / 2 - line_offset);
            Vector3 ArrowTarget = new Vector3();

            renderer.SetPosition(0, ArrowOrigin); // начало каждой линии это начало координат
            switch (i)
            {
                case 0:
                    ArrowTarget = new Vector3(x_count / 2 + line_indent, 0, -z_count / 2 - line_offset);
                    arrow_offset = arrow_proc / x_count;
                    float width = renderer.startWidth;
                    renderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
                    break;
                case 1:
                    ArrowTarget = new Vector3(-x_count / 2 - line_offset, 0, z_count / 2 + line_indent);
                    arrow_offset = arrow_proc / z_count;
                    break;
                case 2:
                    float fake_min = Mathf.Min(y_normalize.ToArray());
                    ArrowTarget = new Vector3(-x_count / 2 - line_offset, height - fake_min + line_indent, -z_count / 2 - line_offset);
                    arrow_offset = arrow_proc / 3; // точки для высоты
                    break;
            }
            renderer.widthCurve = new AnimationCurve(
                new Keyframe(0, 0.2f),
                new Keyframe(0.999f - arrow_offset, 0.2f),  // отрезок отделяющий голову стрелы от линии
                new Keyframe(1f - arrow_offset, 1f),  // самый толстый участок стрелы
                new Keyframe(1, 0f));  // край стрелки

            renderer.SetPosition(1, Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.999f - arrow_offset));
            renderer.SetPosition(2, Vector3.Lerp(ArrowOrigin, ArrowTarget, 1f - arrow_offset));
            renderer.SetPosition(3, ArrowTarget);
        }
    }

    private void Create_graph_object(List<float> x, List<float> z)
    {
        // x - обороты (отсортированы по неубыванию), z - нагрузка

        // int x_size = x_count / 2 - 1;
        // int z_size = z_count / 2 - 1;

        // // опорные точки
        // Vector3[] vertices = new Vector3[(x_size + 1) * (z_size + 1)];

        // for (int i = 0, x = 0; x <= x_size; x++)
        //     for (int z = 0; z <= z_size; z++)
        //     {
        //         vertices[i] = new Vector3(x, y_normalize[i], z);
        //         i++;
        //     }
        // // треугольники между точками
        // int[] triangles = new int[x_size * z_size * 6];
        // int vert = 0;
        // int tris = 0;

        // for (int x = 0; x < x_size; x++)
        // {
        //     for (int z = 0; z < z_size; z++)
        //     {
        //         // создание двух треугольников, образующих квадрат (основной объект)
        //         triangles[tris + 0] = vert + x_size + 2;
        //         triangles[tris + 1] = vert + x_size + 1;
        //         triangles[tris + 2] = vert + 1;
        //         triangles[tris + 3] = vert + 1;
        //         triangles[tris + 4] = vert + x_size + 1;
        //         triangles[tris + 5] = vert;
        //         vert++;
        //         tris += 6;
        //     }
        //     vert++; // удаляет разрыв первого и последнего треугольника
        // }

        // // цвет графика
        // colors = new Color[vertices.Length];
        // float fake_min = Mathf.Min(y_normalize.ToArray());
        // float fake_max = Mathf.Max(y_normalize.ToArray());
        // for (int i = 0, x = 0; x <= x_size; x++)
        //     for (int z = 0; z <= z_size; z++)
        //     {
        //         float height_val = Mathf.InverseLerp(fake_min, fake_max, vertices[i].y);
        //         colors[i] = gradient.Evaluate(height_val);
        //         i++;
        //     }

        int x_size = x.Count;
        int z_size = z.Count;

        vertices = new Vector3[x_size * z_size];
        Vector2[] vertices_2d = new Vector2[x_size * z_size];

        // в каждой ячейке хранится количество одинаковых оборотов
        // каждая ячейка разное число оборотов
        List<int> x_pos = new List<int>(1){1};
        float curr = x[0];
        for (int i = 1, pos = 0; i < x_size; i++)
        {
            if (x[i] == curr)
                x_pos[pos]++;
            else
            {
                x_pos.Add(1);
                pos++;
                curr = x[i];
            }
        }

        for (int i = 0; i < x_pos.Count; i++)
        {
            for (int j = 0; j < x_pos[i]; j++)
            {
                vertices[i] = new Vector3(i, y_normalize[i], j);
                vertices_2d[i] = new Vector2(i, j);
                Debug.Log(vertices_2d[i]);
            }
        }

        //Triangulator tr = new Triangulator(vertices_2d);
        //int[] triangles = tr.Triangulate();

        //mesh.Clear();
        // mesh.subMeshCount = 2; // 2 меша, 0 - основная текстура с градиентом, 1 - черные линии
        //mesh.vertices = vertices;
        //mesh.triangles = triangles;
        // mesh.colors = colors;
        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();

        // mesh.SetIndices(triangles, MeshTopology.Lines, 1);
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
            Gizmos.DrawSphere(vertices[i], 0.1f);
    }
}
