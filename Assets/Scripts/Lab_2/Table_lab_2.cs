using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Table_lab_2 : MonoBehaviour
{
    private RectTransform content;
    private List<GameObject> items_list; // хранение ячеек на сцене
    public GameObject prefab;

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
        transform.Find("Info5").GetComponent<Text>().text = "УОЗ (град)";
        transform.Find("Info6").GetComponent<Text>().text = "Нагрузка (H*м)";

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
        model.num.text = item.items[0];
        model.input1.text = item.items[1];
        model.input2.text = item.items[2];
        model.input3.text = item.items[3];
        model.input4.text = item.items[4];
        model.input5.text = item.items[5];
        items_list.Add(instanse);
    }

    public void AddMany(string[,] items)
    {
        DeleteAll();
        for (int i = 0; i < items.GetLength(0); i++)
        {
            Item item = new Item(new string[] {(i + 1).ToString(), items[i, 0], items[i, 1],
                items[i, 2], items[i, 3], items[i, 4]});
            CreateItem(item);
        }  
    }

    public string[,] GetItems()
    {
        string[,] items = new string[items_list.Count, 5];
        int i = 0;
        foreach (GameObject obj in items_list)
        {
            ItemModel item = new ItemModel(obj.transform);
            items[i, 0] = item.input1.text;
            items[i, 1] = item.input2.text;
            items[i, 2] = item.input3.text;
            items[i, 3] = item.input4.text;
            items[i, 4] = item.input5.text;
            i++;
        }
        return items;
    }

    public struct ItemModel
    {
        public Text num;
        public InputField input1;
        public InputField input2;
        public InputField input3;
        public InputField input4;
        public InputField input5;

        public ItemModel(Transform transform)
        {
            num = transform.Find("Text").GetComponent<Text>();
            input1 = transform.Find("Field_1").GetComponent<InputField>();
            input2 = transform.Find("Field_2").GetComponent<InputField>();
            input3 = transform.Find("Field_3").GetComponent<InputField>();
            input4 = transform.Find("Field_4").GetComponent<InputField>();
            input5 = transform.Find("Field_5").GetComponent<InputField>();
        }
    }

    public struct Item
    {
        public string[] items;

        public Item(string[] nums)
        {
            items = new string[6];
            items = nums;
        }
        public Item(string num)
        {
            items = new string[6];
            items[0] = num;
            for (int i = 1; i < 6; i++)
                items[i] = "0";
        }
    }
}

