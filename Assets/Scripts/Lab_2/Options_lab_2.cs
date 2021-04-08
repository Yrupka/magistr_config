using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;

public class Options_lab_2 : MonoBehaviour
{
    private Table_lab_2 table;
    private Engine_options_lab_2 options;
    private Questions_list questions;
    public Graph_3d graph;

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
        table = transform.Find("Table").GetComponent<Table_lab_2>();
        transform.Find("Table_save").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Save_dialog()));
        transform.Find("Table_load").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Load_dialog()));
        transform.Find("Graph_update").GetComponent<Button>().onClick.AddListener(Graph_update);
        input_m = transform.Find("Input_1").GetComponent<InputField>();
        input_l = transform.Find("Input_2").GetComponent<InputField>();
        input_t = transform.Find("Input_3").GetComponent<InputField>();
        input_inter = transform.Find("Input_4").GetComponent<Slider>();
        // изменение значения слайдера приведет к изменению цифры в окошке
        input_inter.onValueChanged.AddListener((value) =>
            transform.Find("Input_text_4").GetComponent<InputField>().text = value.ToString());
        // обновить все графики, если было изменено значение интерполяции
        input_hints = transform.Find("Input_5").GetComponent<InputField>();
        input_hints.onEndEdit.AddListener(Hints_update);
        input_car = transform.Find("Input_6").GetComponent<InputField>();
        input_eng = transform.Find("Input_7").GetComponent<InputField>();
        hints_dropdown = transform.Find("Dropdown_5").GetComponent<Dropdown>();
        hints_dropdown.onValueChanged.AddListener(Dropdown_updated);
        hints_dropdown.AddOptions(new List<string>()
            { "Подсказка 1", "Подсказка 2", "Подсказка 3", "Подсказка 4"});
        hints_condition = transform.Find("Text_condition_5").GetComponent<Text>();
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
        options.fuel_amount = int.Parse(input_m.text, culture);
        options.lever_length = lever;
        options.heat_time = int.Parse(input_t.text);
        options.Set_rpms(table.GetItems());
        if (options.rpms.Count != 0)
            options.rpms.Sort((a, b) => a.rpm.CompareTo(b.rpm));
        options.interpolation = (int)input_inter.value;
        options.hints = hint_texts;
        options.questions = questions.Get_questions();
        options.max_moment = Mathf.Max((options.Get_list_moment().ToArray()));
    }

    private void Load()
    {
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
    }

    private void Graph_update()
    {
        string[,] data = table.GetItems();
        List<Engine_options_lab_2.struct_rpms> str = new List<Engine_options_lab_2.struct_rpms>(data.GetLength(0));
        
        // сортировка
        for (int i = 0; i < data.GetLength(0); i++)
            str.Add(new Engine_options_lab_2.struct_rpms(int.Parse(data[i, 0]), 0f, 0f,
                float.Parse(data[i, 3]), float.Parse(data[i, 4])));
        if (str.Count != 0)
            str.Sort((a, b) => a.rpm.CompareTo(b.rpm));
        
        // подготовка массив для создания графика
        List<float> rpm = new List<float>();
        List<float> deg = new List<float>();
        List<float> load = new List<float>();
        foreach (Engine_options_lab_2.struct_rpms item in str)
        {
            rpm.Add(item.rpm);
            deg.Add(item.deg);
            load.Add(item.load);
        }
        graph.Create_graph(rpm, deg, load);
    }

    IEnumerator Save_dialog()
	{
        FileBrowser.SetDefaultFilter( ".txt" );
		yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, "таблица_1.txt", "Сохранить файл данных", "Сохранить" );

		if(FileBrowser.Success)
            File_controller.Save_table(table.GetItems(), FileBrowser.Result[0]);
    }

    IEnumerator Load_dialog()
	{
        FileBrowser.SetDefaultFilter( ".txt" );
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Выберите файл для загрузки", "Загрузить" );

		if(FileBrowser.Success)
        {
            string[,] data = File_controller.Load_table(FileBrowser.Result[0], 2);
            if (data != null) // вставить всплывающее окно об ошибке
                table.AddMany(data);
        }
    }

    public void Set_profile(Engine_options_lab_2 profile)
    {
        options = profile;
        Load();
    }

    public Engine_options_lab_2 Get_profile()
    {
        return options;
    }

    public void Add_listener_closed(UnityAction action)
    {
        action_close += action;
    }
}