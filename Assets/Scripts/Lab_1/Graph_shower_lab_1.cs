using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph_shower_lab_1 : MonoBehaviour
{
    private Graph graph;

    private Dropdown dropdown;
    private Transform title;

    private List<float> interpolated_x;
    private List<float>[] graph_data;
    
    private int interpolation;

    private void Awake()
    {
        graph = transform.Find("Graph_window").GetComponent<Graph>();
        dropdown = transform.Find("Graph_list").GetComponent<Dropdown>();
        title = transform.Find("Graph_title");
        dropdown.AddOptions(new List<string>() {
            "Выберите..." , "Крутящий момент",
            "Мощность",  "Часовой расход топлива",
            "Удельный расход топлива" });
        dropdown.onValueChanged.AddListener(Graph_change);
        graph_data = new List<float>[4];
        for (int i = 0; i < 4; i++)
            graph_data[i] = new List<float>();
    }

    private void Graph_change(int value)
    {
        if (value == 0 || graph_data.Length == 0)
            title.gameObject.SetActive(true);
        else
        {
            value--;
            title.gameObject.SetActive(false);
            graph.Show_graph(interpolated_x, graph_data[value], interpolation + 1);
        }
    }

    private void Moment_graphs(List<float> label_x, List<float> moments)
    {
        graph_data[0] = Calculation_formulas.Interpolate_dublicate(
                    label_x, moments, interpolated_x);

        for (int i = 0; i < moments.Count; i++)
            moments[i] *= label_x[i] / 9550f;
        graph_data[1] = Calculation_formulas.Interpolate_dublicate(
            label_x, moments, interpolated_x);
    }

    private void Consumption_graphs(List<float> label_x, List<float> consumptions)
    {
        graph_data[2] = Calculation_formulas.Interpolate_dublicate(
                    label_x, consumptions, interpolated_x);
    }

    private void Moment_and_consumption_graph(List<float> label_x, List<float> moments, List<float> consumptions)
    {
        for (int i = 0; i < moments.Count; i++)
            moments[i] *= label_x[i] / 9550f / consumptions[i] * 3.6f;
        graph_data[3] = Calculation_formulas.Interpolate_dublicate(
            label_x, moments, interpolated_x);
    }

    public void Calculate_graphs(Engine_options_lab_1 options, int graph_num)
    {
        List<float> label_x = options.Get_list_rpm();
        interpolation = options.interpolation;
        interpolated_x = Calculation_formulas.Interpolated_x(
            options.Get_list_rpm(), interpolation);

        switch (graph_num)
        {
            case 0:
                Moment_graphs(label_x, options.Get_list_moment());
                break;

            case 1:
                Consumption_graphs(label_x, options.Get_list_consumption());
                break;

            case 2:
                Moment_graphs(label_x, options.Get_list_moment());
                Consumption_graphs(label_x, options.Get_list_consumption());
                break;   
        }
        Moment_and_consumption_graph(label_x, options.Get_list_moment(), options.Get_list_consumption());
        Graph_change(dropdown.value);
    }

    public void Clear_graphs()
    {
        dropdown.value = 0;
        for (int i = 0; i < 4; i++)
            graph_data[i].Clear();
    }
}
