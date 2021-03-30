using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    private RectTransform graph_container;
    private RectTransform label_template_x;
    private RectTransform label_template_y;
    private RectTransform dash_template_x;
    private RectTransform dash_template_y;
    private List<GameObject> game_object_list;

    public GameObject dot;

    private void Awake()
    {
        graph_container = transform.Find("Graph_conteiner").GetComponent<RectTransform>();
        label_template_x = graph_container.Find("Label_x").GetComponent<RectTransform>();
        label_template_y = graph_container.Find("Label_y").GetComponent<RectTransform>();
        dash_template_x = graph_container.Find("Dash_x").GetComponent<RectTransform>();
        dash_template_y = graph_container.Find("Dash_y").GetComponent<RectTransform>();
        

        game_object_list = new List<GameObject>();
    }

    private void Create_label_x(int val, float pos)
    {
        RectTransform labelX = Instantiate(label_template_x);
        labelX.SetParent(graph_container, false);
        labelX.gameObject.SetActive(true);
        labelX.anchoredPosition = new Vector2(pos, -7f);
        labelX.GetComponent<Text>().text = val.ToString();
        game_object_list.Add(labelX.gameObject);

        RectTransform dashX = Instantiate(dash_template_x);
        dashX.SetParent(graph_container, false);
        dashX.gameObject.SetActive(true);
        dashX.anchoredPosition = new Vector2(pos, -3f);
        game_object_list.Add(dashX.gameObject);
    }

    private void Create_labels_y(float hight, float max)
    {
        float separatorCount = 10;
        for (float i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(label_template_y);
            labelY.SetParent(graph_container, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i / separatorCount;
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * hight);

            labelY.GetComponent<Text>().text = (normalizedValue * max).ToString("0.00");
            game_object_list.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dash_template_y);
            dashY.SetParent(graph_container, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * hight);
            game_object_list.Add(dashY.gameObject);
        }
    }

    private GameObject Create_dot_connection(Vector2 pos_a, Vector2 pos_b)
    {
        GameObject game_object = new GameObject("dot_connection", typeof(Image));
        game_object.transform.SetParent(graph_container, false);
        game_object.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = game_object.GetComponent<RectTransform>();
        Vector2 dir = (pos_b - pos_a).normalized;
        float distance = Vector2.Distance(pos_a, pos_b);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = pos_a + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        rectTransform.SetAsFirstSibling();
        return game_object;
    }

    public void Show_graph(List<int> label_x, List<float> label_y, int split_dot_number)
    {
        int max_visible = label_y.Count;

        foreach (GameObject gameObject in game_object_list)
        {
            Destroy(gameObject);
        }
        game_object_list.Clear();

        float graph_width = graph_container.sizeDelta.x;
        float graph_height = graph_container.sizeDelta.y;

        float max_y = Mathf.Max(label_y.ToArray()) + 1f;

        float size_x = graph_width / (max_visible + 1);

        GameObject last_circle_go = null;
        for (int i = 0, red_dot = split_dot_number; i < max_visible; i++, red_dot++)
        {
            float position_x = size_x + i * size_x;
            float position_y = label_y[i] / max_y * graph_height;

            GameObject circle_go;
            Color color = Color.white;
            if (red_dot == split_dot_number)
            {
                color = Color.red;
                Create_label_x(label_x[i], position_x);
                red_dot = 0;
            }
            circle_go = Instantiate(dot);
            circle_go.GetComponent<Dots>().Set_data(new Vector2(position_x, position_y),
                    graph_container, label_x[i], label_y[i], color);
            circle_go.transform.SetAsFirstSibling();
            game_object_list.Add(circle_go);

            if (last_circle_go != null)
            {
                GameObject dot_connection_go = Create_dot_connection(
                    last_circle_go.GetComponent<RectTransform>().anchoredPosition,
                    circle_go.GetComponent<RectTransform>().anchoredPosition);

                game_object_list.Add(dot_connection_go);
            }
            last_circle_go = circle_go;


        }
        Create_labels_y(graph_height, max_y);
        graph_container.Find("Background").SetAsFirstSibling(); // нужно чтобы фон ничего не загораживал
    }
}
