using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph_3d : MonoBehaviour
{
    private Mesh mesh;
    private Color[] colors;
    private Transform rotator; // объект, который обеспечивает вращение графика

    private List<float> y_normalize;
    private struct dot
    {
        public float x;
        public float y;
    };

    // изменяемые параметры
    private int y_points; // количество промежуточных точек по у (высота)
    private float height; // допустимая высота графика
    private float graph_offset; // расстояние подписей от графика
    private float line_offset; // расстояние линии от графика
    private float step_min_x, step_min_z; // минимальное расстояние между точками
    private string label_x, label_y, label_z; // названия осей

    Vector3[] vertices;

    public Gradient gradient;
    public GameObject text;
    public GameObject line;
    public Camera cam;
    public GameObject cube;

    void Start()
    {
        rotator = transform.parent;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // List<float> x = new List<float>();
        // List<float> random = new List<float>();
        // List<float> z = new List<float>();
        // int x_c = Random.Range(5, 100);
        // for (int i = 0; i < x_c; i++)
        // {
        //     x.Add(1000f * Random.Range(1, 7));
        //     random.Add(Random.Range(0f, 40f));
        //     z.Add(10f * Random.Range(1f, 5f));
        // }

        // x.Sort();

        y_points = 2;
        y_points += 2; // минимально должны быть точка минимума и максимума
        height = 3f;
        graph_offset = 0.5f;
        line_offset = 0.4f;
        step_min_x = 0.1f;
        step_min_z = 0.1f;
        label_x = "Обороты";
        label_y = "Угол опережения";
        label_z = "Нагрузка";

        //Create_graph(x, random, z);
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
        y_normalize = new List<float>();
        float max_y = Mathf.Max(y.ToArray());
        float min_y = Mathf.Min(y.ToArray());

        foreach (float item in y)
            y_normalize.Add(item / max_y * height);

        float min = Mathf.Min(y_normalize.ToArray());
        for (int i = 0; i < y_normalize.Count; i++)
            y_normalize[i] -= min;

        // количество точек по осям без дубликатов
        List<float> converted_x = x.Distinct().ToList();
        List<float> converted_z = z.Distinct().ToList();
        float x_count = converted_x.Count;
        float z_count = converted_z.Count;

        // расчет шага
        int max_length_x = 10, max_length_z = 10; // максимально возможная длина оси для точек
        bool length_x = false, length_z = false;
        while (!length_x || !length_z)
        {
            if (max_length_x / x_count < step_min_x)
                max_length_x += 10;
            else
                length_x = true;

            if (max_length_z / z_count < step_min_z)
                max_length_z += 10;
            else
                length_z = true;
        }

        Clear_objects();
        Create_graph_object(x, z, max_length_x, max_length_z);

        Set_position(max_length_x, max_length_z);
        Create_labels(
            converted_x.Min(), converted_x.Max(),
            min_y, max_y,
            converted_z.Min(), converted_z.Max(),
            max_length_x, max_length_z);
        Create_lines(max_length_x, max_length_z);
    }

    private void Clear_objects() // удаляет все объекты предыдущего графика
    {
        for (int i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    // опускает график, так чтобы минимальная точка лежала на высоте 0
    // настраевает камеру, для корректного отображения
    private void Set_position(int length_x, int length_z)
    {
        float fake_min = Mathf.Min(y_normalize.ToArray());
        transform.localPosition = new Vector3(-length_x / 2, -fake_min, -length_z / 2);
        cam.transform.localPosition = new Vector3(-length_x / 2 - 2, height / 2, -length_z / 2 - 2);
        cam.transform.rotation = Quaternion.Euler(new Vector3(15f, 45f, 0f));
        // плоскость, чтобы заслонять ненужные части графика
        cube.transform.localScale = new Vector3(length_x, 1, length_z);
        cube.transform.localPosition = new Vector3(-line_offset, -fake_min - 0.5f, -line_offset);
    }

    // создает подписи осей
    private void Create_labels(
        float min_x, float max_x,
        float min_y, float max_y,
        float min_z, float max_z,
        int length_x, int length_z)
    {
        // x
        GameObject lab_x = Instantiate(text, transform);
        lab_x.GetComponent<TextMesh>().text = label_x;
        lab_x.GetComponent<TextMesh>().anchor = TextAnchor.UpperLeft;
        lab_x.transform.localPosition = new Vector3(1f, 0f, -(line_offset + graph_offset));

        for (int i = 0; i < length_x; i++)
        {
            GameObject instanse = Instantiate(text, transform);
            instanse.transform.localPosition = new Vector3(i, 0, -graph_offset);
            instanse.GetComponent<TextMesh>().text = Mathf.Lerp(min_x, max_x, (float)i / (length_x - 1)).ToString("#0.0");
        }

        //z
        GameObject lab_z = Instantiate(text, transform);
        lab_z.GetComponent<TextMesh>().text = label_z;
        lab_z.GetComponent<TextMesh>().anchor = TextAnchor.UpperRight;
        lab_z.transform.localPosition = new Vector3(-(line_offset + graph_offset), 0f, 1f);
        lab_z.transform.Rotate(Vector3.up, 90);

        for (int i = 0; i < length_z; i++)
        {
            GameObject instanse = Instantiate(text, transform);
            instanse.transform.Rotate(Vector3.up, 90);
            instanse.transform.localPosition = new Vector3(-graph_offset, 0, i);
            instanse.GetComponent<TextMesh>().text = Mathf.Lerp(min_z, max_z, (float)i / (length_z - 1)).ToString("#0.00");
        }

        //y
        float y_offset = Mathf.Sqrt(Mathf.Pow(graph_offset, 2) * 2f) / 1.5f; // отступ от графика на расстояние гипотенузы треугольника со сторонами отступа
        GameObject lab_y = Instantiate(text, transform);
        lab_y.GetComponent<TextMesh>().text = label_y;
        lab_y.transform.localPosition = new Vector3(-(y_offset + 0.2f), 0.5f, -(y_offset + 0.2f));
        lab_y.transform.Rotate(Vector3.up, 45);

        float step = height / y_points;
        float procent = 1f / (y_points - 1);
        for (int i = 0; i < y_points; i++)
        {
            GameObject instanse = Instantiate(text, transform);
            instanse.transform.Rotate(Vector3.up, 45);
            Vector3 position = new Vector3(-y_offset, 0, -y_offset);

            position.y = i * step + instanse.transform.localScale.y * 1.5f; // + половина высоты текста
            instanse.transform.localPosition = position;
            instanse.GetComponent<TextMesh>().text = Mathf.Lerp(min_y, max_y, (float)i * procent).ToString("#0.00");
        }
    }

    //создает линии для осей
    private void Create_lines(int length_x, int length_z)
    {
        float arrow_proc = 0.25f; // процент, который будет занимать наконечник стелы от длины линии | наконечник стрелки занимает 1/4 от 1
        float line_indent = 0.5f; // отступ линии от последней точки значений
        for (int i = 0; i < 3; i++)
        {
            GameObject instanse = Instantiate(line, transform);
            LineRenderer renderer = instanse.GetComponent<LineRenderer>();
            renderer.positionCount = 4; // количество отрезков в линии
            float arrow_offset = 0f; // 
            Vector3 ArrowOrigin = new Vector3(-line_offset, 0, -line_offset);
            Vector3 ArrowTarget = new Vector3();

            renderer.SetPosition(0, ArrowOrigin); // начало каждой линии это начало координат
            switch (i)
            {
                case 0:
                    ArrowTarget = new Vector3(length_x + line_indent, 0, -line_offset);
                    arrow_offset = arrow_proc / length_x;
                    float width = renderer.startWidth;
                    renderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
                    break;
                case 1:
                    ArrowTarget = new Vector3(-line_offset, 0, length_z + line_indent);
                    arrow_offset = arrow_proc / length_z;
                    break;
                case 2:
                    float fake_min = y_normalize.Min();
                    ArrowTarget = new Vector3(-line_offset, height - fake_min + line_indent, -line_offset);
                    arrow_offset = arrow_proc / y_points; // точки для высоты
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

    private void Create_graph_object(List<float> x, List<float> z, int length_x, int length_z)
    {
        // x - обороты (отсортированы по неубыванию), z - нагрузка

        int diff_dots = x.Distinct().Count();
        List<dot>[] dots = new List<dot>[diff_dots + 1];
        for (int i = 0; i < diff_dots + 1; i++)
            dots[i] = new List<dot>();

        int max_dot_count = 0;
        float curr = x[0];

        // распределение точек на каждое отдельное число оборотов
        for (int i = 0, pos = 0; i < x.Count; i++)
        {
            if (x[i] != curr)
            {
                pos++;
                curr = x[i];
                if (dots[pos - 1].Count > max_dot_count)
                    max_dot_count = dots[pos - 1].Count;
            }
            dots[pos].Add(new dot() { x = z[i], y = y_normalize[i]});
        }

        // сортировка по возрастанию полученных точек
        for (int i = 0; i < diff_dots; i++)
            dots[i].Sort((a, b) => a.x.CompareTo(b.x));

        // добавление дополнительных точек для получения равных массивов
        for (int i = 0; i < diff_dots; i++)
        {
            int dots_to_add = max_dot_count - dots[i].Count;
            for (int j = 0; j < dots_to_add; j++)
                dots[i].Add(new dot() { x = dots[i].Last().x, y = dots[i].Last().y });
            dots[i].Add(new dot() { x = dots[i].Last().x, y = dots[i].Last().y });
        }
        dots.Last().AddRange(dots[diff_dots - 1]);

        int vert_size = (diff_dots + 1) * (max_dot_count + 1);
        vertices = new Vector3[vert_size];
        List<float> x_new = x.Distinct().ToList();
        x_new.Add(x_new.Last()); // дубль последнего

        // создание опорных точек для графика
        for (int i = 0, count = 0; i <= diff_dots; i++)
        {
            for (int j = 0; j < dots[i].Count; j++)
            {
                if (count > vert_size)
                    break;
                float zz = Mathf.InverseLerp(dots[i].Min((a) => a.x), dots[i].Max((a) => a.x), dots[i][j].x);
                float xx = Mathf.InverseLerp(x_new.Min(), x_new.Max(), x_new[i]);
                vertices[count] = new Vector3(Mathf.Lerp(0f, length_x - 1, xx), dots[i][j].y, Mathf.Lerp(0f, length_z - 1, zz));
                count++;
            }
        }

        // треугольники между точками
        int[] triangles = new int[max_dot_count * diff_dots * 6];
        int vert = 0;
        int tris = 0;

        for (int i = 0; i < diff_dots; i++)
        {
            for (int j = 0; j < max_dot_count; j++)
            {
                // создание двух треугольников, образующих квадрат (основной объект)
                triangles[tris + 0] = vert + 1;
                triangles[tris + 1] = vert + max_dot_count + 1;
                triangles[tris + 2] = vert;
                triangles[tris + 3] = vert + max_dot_count + 1;
                triangles[tris + 4] = vert + 1;
                triangles[tris + 5] = vert + max_dot_count + 2;
                vert++;
                tris += 6;
            }
            vert++; // удаляет разрыв первого и последнего треугольника
        }

        // // цвет графика
        colors = new Color[vertices.Length];
        float fake_min = Mathf.Min(y_normalize.ToArray());
        float fake_max = Mathf.Max(y_normalize.ToArray());
        for (int i = 0, count = 0; i <= diff_dots; i++)
            for (int j = 0; j <= max_dot_count; j++)
            {
                float height_val = Mathf.InverseLerp(fake_min, fake_max, vertices[count].y);
                colors[count] = gradient.Evaluate(height_val);
                count++;
            }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.Optimize();
    }
}
