using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SimpleFileBrowser;


public class Options_lab_1 : MonoBehaviour
{
    private Table_lab_1 table;
    private Engine_options_lab_1 options;
    private Graph_shower graph;
    private Questions_list questions;

    private InputField input_m; // масса добавляемого топлива
    private InputField input_l; // длина рычага тормозящего устройства
    private InputField input_t; // время нагрева двигателя до рабочей температуры
    private Slider input_inter; // количество точек интерполяции
    private Dropdown hints_dropdown; // список подсказок
    private Text hints_condition;
    private InputField input_hints; // подсказки
    private InputField input_car; // имя двигателя
    private InputField input_eng; // имя машины
    // для разшения формата float с точкой вместо запятой
    System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
    private UnityAction action_close;

    private string[] hint_texts;
    private string[] conditions;

    private void Awake() // нахождение всех полей
    {
        table = transform.Find("Table").GetComponent<Table_lab_1>();
        transform.Find("Table_save").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Save_dialog()));
        transform.Find("Table_load").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Load_dialog()));
        table.Add_listener_update_first(() => Graph_update(2));
        table.Add_listener_update_second(() => Graph_update(0));
        table.Add_listener_update_third(() => Graph_update(1));
        input_m = transform.Find("Input_1").GetComponent<InputField>();
        input_l = transform.Find("Input_2").GetComponent<InputField>();
        input_t = transform.Find("Input_3").GetComponent<InputField>();
        input_inter = transform.Find("Input_4").GetComponent<Slider>();
        // изменение значения слайдера приведет к изменению цифры в окошке
        input_inter.onValueChanged.AddListener((value) =>
            transform.Find("Input_text_4").GetComponent<InputField>().text = value.ToString());
        // обновить все графики, если было изменено значение интерполяции
        input_inter.onValueChanged.AddListener((value) => Graph_update(2));
        input_hints = transform.Find("Input_5").GetComponent<InputField>();
        input_hints.onEndEdit.AddListener(Hints_update);
        input_car = transform.Find("Input_6").GetComponent<InputField>();
        input_eng = transform.Find("Input_7").GetComponent<InputField>();
        hints_dropdown = transform.Find("Dropdown_5").GetComponent<Dropdown>();
        hints_dropdown.onValueChanged.AddListener(Dropdown_updated);
        hints_dropdown.AddOptions(new List<string>()
            { "Подсказка 1", "Подсказка 2", "Подсказка 3", "Подсказка 4"});
        hints_condition = transform.Find("Text_condition_5").GetComponent<Text>();
        graph = transform.Find("Graph").GetComponent<Graph_shower>();
        transform.Find("Save_button").GetComponent<Button>().onClick.AddListener(Save);
        transform.Find("Load_button").GetComponent<Button>().onClick.AddListener(Load);
        transform.Find("Exit_button").GetComponent<Button>().onClick.AddListener(
            () => transform.parent.parent.gameObject.SetActive(false));
        hint_texts = new string[4];
        conditions = new string[4] {"Вход в лабораторию", "Тара на весах",
            "Топливоприемник опущен", "Двигатель работает"};
        questions = transform.Find("Questions").GetComponent<Questions_list>();
    }

    private void OnDisable()
    {
        action_close?.Invoke();
    }

    private void Dropdown_updated(int value)
    {
        input_hints.text = hint_texts[value];
        hints_condition.text = conditions[value];
    }

    private void Hints_update(string value)
    {
        hint_texts[hints_dropdown.value] = value;
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(input_m.text)) input_m.text = "0";
        if (string.IsNullOrEmpty(input_l.text)) input_l.text = "0";
        if (string.IsNullOrEmpty(input_t.text)) input_t.text = "0";
        if (string.IsNullOrWhiteSpace(input_car.text)) input_car.text = "м";
        if (string.IsNullOrWhiteSpace(input_eng.text)) input_eng.text = "д";

        options.car_name = input_car.text;
        options.engine_name = input_eng.text;
        float lever = float.Parse(input_l.text, culture);
        if (lever == 0)
            lever = 1;
        int fuel_amount = int.Parse(input_m.text, culture);
        if (fuel_amount == 0)
            fuel_amount = 1;
        int heat_time = int.Parse(input_t.text);
        if (heat_time == 0)
            heat_time = 1;
        options.fuel_amount = fuel_amount;
        options.lever_length = lever;
        options.heat_time = heat_time;
        options.Set_rpms(table.GetItems());
        if (options.rpms.Count != 0)
            options.rpms.Sort((a, b) => a.rpm.CompareTo(b.rpm));
        options.interpolation = (int)input_inter.value;
        options.hints = hint_texts;
        options.max_moment = graph.Get_max_moment();
        options.questions = questions.Get_questions();
    }

    private void Load()
    {
        graph.Clear_graphs();

        hints_dropdown.value = 0;
        hint_texts = options.hints;
        input_car.text = options.car_name;
        input_eng.text = options.engine_name;
        input_m.text = options.fuel_amount.ToString(culture);
        input_l.text = options.lever_length.ToString(culture);
        input_t.text = options.heat_time.ToString(culture);
        table.AddMany(options.Get_rpms());
        input_inter.value = options.interpolation;
        input_hints.text = hint_texts[hints_dropdown.value];
        hints_condition.text = conditions[hints_dropdown.value];
        questions.Set_questions(options.questions);
        Graph_update(2);
    }

    // обновить график по номеру (0-момента, мощности, 1-расхода, удельного расхода, 2 - обновить все)
    private void Graph_update(int graph_num)
    {
        Engine_options_lab_1 graph_options = new Engine_options_lab_1("", "");
        graph_options.Set_rpms(table.GetItems());
        graph_options.interpolation = (int)input_inter.value;
        if (graph_options.rpms.Count != 0)
        {
            graph_options.rpms.Sort((a, b) => a.rpm.CompareTo(b.rpm));
            // удаление дубликатов
            for (int i = 0; i < graph_options.rpms.Count - 1; i++)
                if (graph_options.rpms[i].rpm == graph_options.rpms[i + 1].rpm)
                {
                    graph_options.rpms.RemoveAt(i + 1);
                    i--;
                }
            graph.Calculate_graphs(graph_options, graph_num);
        }
    }

    IEnumerator Save_dialog()
    {
        FileBrowser.SetDefaultFilter(".txt");
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, "таблица_1.txt", "Сохранить файл данных", "Сохранить");

        if (FileBrowser.Success)
            File_controller.Save_table(table.GetItems(), FileBrowser.Result[0]);
    }

    IEnumerator Load_dialog()
    {
        FileBrowser.SetDefaultFilter(".txt");
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Выберите файл для загрузки", "Загрузить");

        if (FileBrowser.Success)
        {
            string[,] data = File_controller.Load_table(FileBrowser.Result[0], 1);
            if (data != null) // вставить всплывающее окно об ошибке
                table.AddMany(data);
        }
    }

    public void Set_profile(Engine_options_lab_1 profile)
    {
        options = profile;
        Load();
    }

    public Engine_options_lab_1 Get_profile()
    {
        return options;
    }

    public void Add_listener_closed(UnityAction action)
    {
        action_close += action;
    }
}