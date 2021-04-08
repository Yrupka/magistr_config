using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.SceneManagement;

public class Profile_system_lab_1 : MonoBehaviour
{
    private List<Engine_options_lab_1> options;
    private int choosed_profile;
    public Options_lab_1 engine_options;

    private Transform profiles_list;
    private Text profile_current;
    private Button btn_delete;
    private Button btn_change;
    private Button btn_choose;
    private List<Toggle> items_list;
    public GameObject profile;


    private void Awake()
    {
        profiles_list = transform.Find("Scroll").Find("Scroll_window").Find("Content");
        profile_current = transform.Find("Text_current").GetComponent<Text>();
        transform.Find("Add").GetComponent<Button>().onClick.AddListener(Profile_add);
        btn_delete = transform.Find("Delete").GetComponent<Button>();
        btn_delete.onClick.AddListener(Profile_delete);
        btn_change = transform.Find("Change").GetComponent<Button>();
        btn_change.onClick.AddListener(Profile_change);
        btn_choose = transform.Find("Choose").GetComponent<Button>();
        btn_choose.onClick.AddListener(Profile_choose);
        transform.Find("Get").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Get_file()));
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        items_list = new List<Toggle>();
        FileBrowser.SetFilters( false, new FileBrowser.Filter( "Таблицы", ".txt"), new FileBrowser.Filter( "Профиль", ".json") );

        Profiles_load();
    }

    private void Start() // необходимо для инициализации окна с настройками параметров
    {
        engine_options.transform.parent.parent.gameObject.SetActive(false);
        engine_options.Add_listener_closed(Profile_update);
    }

    private void OnDestroy()
    {
        File_controller.Save(choosed_profile, options);
    }

    private void Profiles_load()
    {
        (options, choosed_profile) = File_controller.Load_lab_1();
        if (options != null)
        {
            foreach (Engine_options_lab_1 item in options)
                Item_add(item.car_name + "-" + item.engine_name);
            Data_update(choosed_profile);
        }
        else
            options = new List<Engine_options_lab_1>();
    }

    private void Profile_add()
    {
        string name_car = transform.Find("Name_car").GetComponent<InputField>().text;
        string name_engine = transform.Find("Name_engine").GetComponent<InputField>().text;
        if (string.IsNullOrWhiteSpace(name_car))
            name_car = "м";
        if (string.IsNullOrWhiteSpace(name_engine))
            name_engine = "д";
        Item_add(name_car + "-" + name_engine);
        options.Add(new Engine_options_lab_1(name_engine, name_car));
        Data_update(choosed_profile);
    }

    private void Item_add(string name)
    {
        GameObject instanse = Instantiate(profile, profiles_list);
        instanse.transform.Find("Label").GetComponent<Text>().text = name;
        Toggle toggle = instanse.GetComponent<Toggle>();
        toggle.group = profiles_list.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener(Selected);
        int index = items_list.Count;
        items_list.Add(toggle);
    }

    private void Profile_delete()
    {
        int current_profile_index = items_list.FindIndex(x => x.isOn == true);
        Destroy(items_list[current_profile_index].gameObject);
        items_list.RemoveAt(current_profile_index);
        options.RemoveAt(current_profile_index);
        if (choosed_profile == current_profile_index)
            choosed_profile = -1;
        if (choosed_profile > current_profile_index)
            choosed_profile--;
        Data_update(choosed_profile);

    }

    private void Profile_change()
    {
        int current_profile_index = items_list.FindIndex(x => x.isOn == true);
        engine_options.Set_profile(options[current_profile_index]);
        engine_options.transform.parent.parent.gameObject.SetActive(true);
        transform.parent.parent.gameObject.SetActive(false);
    }

    private void Profile_choose()
    {
        choosed_profile = items_list.FindIndex(x => x.isOn == true);
        profile_current.text = options[choosed_profile].car_name + "-" + options[choosed_profile].engine_name;
    }

    private void Profile_update()
    {
        int current_profile_index = items_list.FindIndex(x => x.isOn == true);
        transform.parent.parent.gameObject.SetActive(true);
        options[current_profile_index] = engine_options.Get_profile();
        items_list[current_profile_index].transform.Find("Label").GetComponent<Text>().text =
            options[current_profile_index].car_name + "-" + options[current_profile_index].engine_name;
    }

    private void Selected(bool state)
    {
        if (profiles_list.GetComponent<ToggleGroup>().AnyTogglesOn())
        {
            btn_delete.interactable = true;
            btn_change.interactable = true;
            btn_choose.interactable = true;

        }
        else
        {
            btn_delete.interactable = false;
            btn_change.interactable = false;
            btn_choose.interactable = false;
        }
    }

    private void Data_update(int index)
    {
        if (index == -1)
            profile_current.text = "Выберите профиль";
        else
            profile_current.text = options[index].car_name + "-" + options[index].engine_name;
    }

    private void Exit()
    {
        SceneManager.LoadScene("Main_menu");
    }

    IEnumerator Get_file()
    {
        FileBrowser.SetDefaultFilter(".json");
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, "profile.json", "Сохранить файл настроек профиля", "Сохранить");

        if (FileBrowser.Success)
            File_controller.Create_json(options[choosed_profile], FileBrowser.Result[0]);
    }
}
