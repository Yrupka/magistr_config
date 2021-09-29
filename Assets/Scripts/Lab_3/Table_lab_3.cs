using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Table_lab_3 : MonoBehaviour
{
    private RectTransform content;
    private List<GameObject> items_list; // хранение ячеек на сцене
    public GameObject prefab;

    private UnityAction action_first;
    private UnityAction action_second;
    private UnityAction action_third;
    private UnityAction action_fourth;

    private void Awake()
    {
        Transform window = transform.Find("Scroll_window");
        content = window.Find("Content").GetComponent<RectTransform>();
        transform.Find("Add_btn").GetComponent<Button>()
            .onClick.AddListener(() => AddItem());
        transform.Find("Del_btn").GetComponent<Button>()
            .onClick.AddListener(() => DeleteItem());
        transform.Find("Info1").GetComponent<Text>().text = "Номер";
        transform.Find("Info2").GetComponent<Text>().text = "Обороты (об/мин)";
        transform.Find("Info3").GetComponent<Text>().text = "Часовой расход (гр/час)";
        transform.Find("Info4").GetComponent<Text>().text = "Момент (H*м)";
        transform.Find("Info5").GetComponent<Text>().text = "Расход воздуха (л/мин)";

        items_list = new List<GameObject>();
    }

    private void DeleteAll() // удаление всех объектов списка
    {
        foreach (GameObject go in items_list)
            Destroy(go);
        items_list = new List<GameObject>();
    }

    private void AddItem()
    {
        Item item = new Item((items_list.Count + 1).ToString());
        CreateItem(item);
    }

    private void DeleteItem()
    {
        int index = items_list.Count;
        if (index != 0)
        {
            Destroy(items_list[index - 1]);
            items_list.RemoveAt(index - 1);
        }
    }

    private void CreateItem(Item item)
    {
        GameObject instanse = Instantiate(prefab); // клонируем префаб
        instanse.transform.SetParent(content, false); // устанавливаем клону родителя
        ItemModel model = new ItemModel(instanse.transform); // создаем по клону объект
        model.num.text = item.number;
        model.input1.text = item.first;
        model.input1.onEndEdit.AddListener((val) => action_first());
        model.input2.text = item.second;
        model.input2.onEndEdit.AddListener((val) => action_second());
        model.input3.text = item.third;
        model.input3.onEndEdit.AddListener((val) => action_third());
        model.input4.text = item.fourth;
        model.input4.onEndEdit.AddListener((val) => action_fourth());
        items_list.Add(instanse);
    }

    public void AddMany(string[,] items)
    {
        DeleteAll();
        for (int i = 0; i < items.GetLength(0); i++)
        {
            Item item = new Item((i + 1).ToString(), items[i, 0], items[i, 1], items[i, 2], items[i, 3]);
            CreateItem(item);
        }  
    }

    public string[,] GetItems()
    {
        string[,] items = new string[items_list.Count, 4];
        int i = 0;
        foreach (GameObject obj in items_list)
        {
            ItemModel item = new ItemModel(obj.transform);
            items[i, 0] = item.input1.text;
            items[i, 1] = item.input2.text;
            items[i, 2] = item.input3.text;
            items[i, 3] = item.input4.text;
            i++;
        }
        return items;
    }

    public void Add_listener_update_first(UnityAction action)
    {
        action_first += action;
    }

    public void Add_listener_update_second(UnityAction action)
    {
        action_second += action;
    }

    public void Add_listener_update_third(UnityAction action)
    {
        action_third += action;
    }

    public void Add_listener_update_fourth(UnityAction action)
    {
        action_fourth += action;
    }

    public struct ItemModel
    {
        public Text num;
        public InputField input1;
        public InputField input2;
        public InputField input3;
        public InputField input4;

        public ItemModel(Transform transform)
        {
            num = transform.Find("Text").GetComponent<Text>();
            input1 = transform.Find("Field_1").GetComponent<InputField>();
            input2 = transform.Find("Field_2").GetComponent<InputField>();
            input3 = transform.Find("Field_3").GetComponent<InputField>();
            input4 = transform.Find("Field_4").GetComponent<InputField>();
        }
    }

    public struct Item
    {
        public string number;
        public string first;
        public string second;
        public string third;
        public string fourth;

        public Item(string par1, string par2, string par3, string par4, string par5)
        {
            number = par1;
            first = par2;
            second = par3;
            third = par4;
            fourth = par5;
        }
        public Item(string num)
        {
            number = num;
            first = "0";
            second = "0";
            third = "0";
            fourth = "0";
        }
    }
}

