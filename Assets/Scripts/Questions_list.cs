using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;

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
        transform.Find("Questions_save").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Save_dialog()));
        transform.Find("Questions_load").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Load_dialog()));
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

    IEnumerator Save_dialog()
	{
        FileBrowser.SetDefaultFilter( ".txt" );
		yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, "Вопросы_1.txt", "Сохранить файл данных", "Сохранить" );

		if(FileBrowser.Success)
        {
            List<Questions_data> questions = Get_questions();
            string[,] data = new string[questions.Count, 3];
            for (int i = 0; i < questions.Count; i++)
            {
                data[i, 0] = questions[i].score.ToString();
                data[i, 1] = questions[i].text;
                string answers = "";
                questions[i].answers.ForEach((x) => answers +=x + '\n');
                data[i, 2] = answers;
            }
            File_controller.Save_questions(data, FileBrowser.Result[0]);
        } 
    }

    IEnumerator Load_dialog()
	{
        FileBrowser.SetDefaultFilter( ".txt" );
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Выберите файл для загрузки", "Загрузить" );

		if(FileBrowser.Success)
        {
            List<Questions_data> questions = new List<Questions_data>();
            string[,] data = File_controller.Load_questions(FileBrowser.Result[0]);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                Questions_data item = new Questions_data();
                item.score = int.Parse(data[i, 0]);
                item.text = data[i, 1];
                if (data[i, 2] != null)
                {
                    item.answers.AddRange(data[i, 2].Split('\n'));
                    item.answers.RemoveAt(item.answers.Count - 1);
                }
                questions.Add(item);
            }
            Set_questions(questions);
        }
    }
}
