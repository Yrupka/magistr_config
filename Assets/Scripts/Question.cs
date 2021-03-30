using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    private GameObject answer;
    private InputField text;
    private Transform answers_base;
    private InputField score;

    private List<GameObject> answers_list; // список не самих вопросов а toggle внутри

    public void Init()
    {
        answers_list = new List<GameObject>();
        answer = transform.Find("Answers").Find("Viewport").Find("Content").Find("Answer").gameObject; // шаблон ответа
        answers_base = answer.transform.parent;
        transform.Find("Add").GetComponent<Button>().onClick.AddListener(Add);
        transform.Find("Delete").GetComponent<Button>().onClick.AddListener(Delete);
        score = transform.Find("Score").GetComponent<InputField>();
        text = transform.Find("Text").GetComponent<InputField>();
    }

    private void Add()
    {
        GameObject answ = Instantiate(answer, answers_base);
        Transform toggle = answ.transform.Find("Toggle");
        answ.SetActive(true);
        toggle.Find("Label").GetComponent<Text>().text = (answers_list.Count + 1).ToString();
        answers_list.Add(toggle.gameObject);
    }

    private void Delete()
    {
        int index = answers_list.FindIndex(x => x.GetComponent<Toggle>().isOn == true);
        if (index == -1) // если ничего не выбрано -> удалить последний
            index = answers_list.Count - 1;
        Destroy(answers_list[index].transform.parent.gameObject); // удалить сам ответ а не toggle
        answers_list.RemoveAt(index);
    }

    public (string, List<string>, int) Get_data()
    {
        string str = text.text;
        int size = answers_list.Count;
        List<string> answers = new List<string>();
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
                answers.Add(answers_list[i].transform.parent.Find("Text").GetComponent<InputField>().text);
        }
        int score_num = string.IsNullOrEmpty(score.text) ? 0 : int.Parse(score.text);
        return (str, answers, score_num);
    }

    public void Set_data(string text_info, List<string> answers, int score_num)
    {
        Init();
        text.text = text_info;
        for (int i = 0; i < answers.Count; i++)
        {
            Add();
            answers_list[i].transform.parent.Find("Text").GetComponent<InputField>().text = answers[i];
        }
        score.text = score_num.ToString();
    }
}
