using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Questions_list : MonoBehaviour
{
    private Transform questions_base;
    public GameObject question;

    private List<GameObject> qustions_list;

    private void Awake()
    {
        qustions_list = new List<GameObject>();
        questions_base = transform.Find("Viewport").Find("Content");
        transform.Find("Add_button").GetComponent<Button>().onClick.AddListener(Add_question);
        transform.Find("Delete_button").GetComponent<Button>().onClick.AddListener(Delete_question);
    }

    private void Add_question()
    {
        GameObject quest = Instantiate(question, questions_base);
        quest.GetComponent<Question>().Init();
        qustions_list.Add(quest);
    }

    private void Delete_question()
    {
        int index = qustions_list.Count - 1;
        Destroy(qustions_list[index]);
        qustions_list.RemoveAt(index);
    }

    public List<Questions_data> Get_questions()
    {
        List<Questions_data> questions = new List<Questions_data>();
        foreach (GameObject item in qustions_list)
        {
            Questions_data data = new Questions_data();
            (data.text, data.answers, data.score) = item.GetComponent<Question>().Get_data();
            if (data.answers.Count > 0)
                data.num = 0;
            else
                data.num = -1;
            questions.Add(data);
        }
        return questions;
    }

    public void Set_questions(List<Questions_data> data)
    {
        for (int i = 0; i < qustions_list.Count; i++)
            Destroy(qustions_list[i]);
        qustions_list.Clear();
        
        for (int i = 0; i < data.Count; i++)
        {
            Add_question();
            qustions_list[i].GetComponent<Question>().Set_data(data[i].text, data[i].answers, data[i].score);
        }
    }
}
