using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class File_controller
{
    [System.Serializable]
    public class json_format
    {
        [SerializeField] private int _profile;
        [SerializeField] private Engine_options_lab_1[] _options_lab_1;
        [SerializeField] private Engine_options_lab_2[] _options_lab_2;
        public json_format(int profile, Engine_options_lab_1[] options)
        {
            _profile = profile;
            _options_lab_1 = options;
        }
        public json_format(int profile, Engine_options_lab_2[] options)
        {
            _profile = profile;
            _options_lab_2 = options;
        }
        public Engine_options_lab_1[] get_options_lab_1()
        {
            return _options_lab_1 == null ? null : _options_lab_1;
        }
        public Engine_options_lab_2[] get_options_lab_2()
        {
            return _options_lab_2 == null ? null : _options_lab_2;
        }
        public int get_profile()
        {
            return _profile;
        }
    }

    public static void Save(int profile, List<Engine_options_lab_1> saveClass)
    {
        json_format json = new json_format(profile, saveClass.ToArray());
        string data = JsonUtility.ToJson(json);
        File.WriteAllText(Application.dataPath + "\\data_1.json", data);
    }

    public static void Save(int profile, List<Engine_options_lab_2> saveClass)
    {
        json_format json = new json_format(profile, saveClass.ToArray());
        string data = JsonUtility.ToJson(json);
        File.WriteAllText(Application.dataPath + "\\data_2.json", data);
    }

    public static void Create_json(Engine_options_lab_1 options, string path)
    {
        string data = JsonUtility.ToJson(options);
        File.WriteAllText(path, data);
    }

    public static void Create_json(Engine_options_lab_2 options, string path)
    {
        string data = JsonUtility.ToJson(options);
        File.WriteAllText(path, data);
    }

    public static (List<Engine_options_lab_1>, int) Load_lab_1()
    {
        if (!File.Exists(Application.dataPath + "\\data_1.json"))
        {
            return (null, -1);
        }
        string data = File.ReadAllText(Application.dataPath + "\\data_1.json");
        json_format json = JsonUtility.FromJson<json_format>(data);
        List<Engine_options_lab_1> options = new List<Engine_options_lab_1>();
        Engine_options_lab_1[] opt = json.get_options_lab_1();
        if (opt != null)
            options.AddRange(json.get_options_lab_1());
        return (options, json.get_profile());
    }

    public static (List<Engine_options_lab_2>, int) Load_lab_2()
    {
        if (!File.Exists(Application.dataPath + "\\data_2.json"))
        {
            return (null, -1);
        }
        string data = File.ReadAllText(Application.dataPath + "\\data_2.json");
        json_format json = JsonUtility.FromJson<json_format>(data);
        List<Engine_options_lab_2> options = new List<Engine_options_lab_2>();
        Engine_options_lab_2[] opt = json.get_options_lab_2();
        if (opt != null)
            options.AddRange(json.get_options_lab_2());
        return (options, json.get_profile());
    }

    public static void Save_table(string[,] data, string path)
    {
        string str = "Обороты\tМомент\tРасход\t";
        int count_rows = data.GetLength(1);

        if (count_rows == 5) // таблица второй лабораторной, 5 строк
            str += "Угол\tНагрузка\n";
        
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < count_rows - 1; j++)
                str += data[i, j] + '\t';
            str += data[i, count_rows - 1] + '\n';
        }
        File.WriteAllText(path, str, Encoding.Unicode);
    }

    public static string[,] Load_table(string path, int lab_num)
    {
        string[] strings = File.ReadAllLines(path, Encoding.Unicode);
        int count_rows;

        if (lab_num == 1)
            count_rows = 3;
        else
            count_rows = 5;

        string[,] data = new string[strings.Length - 1, count_rows];

        // на первом месте надписи
        for (int i = 1; i < strings.Length; i++)
        {
            string[] str = strings[i].Split('\t');
            if (str.Length != count_rows) // пропускает не корректные строки
                continue;
            for (int j = 0; j < str.Length; j++)
                data[i - 1, j] = str[j];
        }
        return data;
    }

    public static void Save_questions(string[,] data, string path)
    {
        string str = "Номер\nБалл\nТекст\nОтветы\n";
        for (int i = 0; i < data.GetLength(0); i++)
        {
            str += (i + 1).ToString() + '\n';
            for (int j = 0; j < 3; j++)
                str += data[i, j] + '\n';
        }
        File.WriteAllText(path, str, Encoding.Unicode);
    }

    public static string[,] Load_questions(string path)
    {
        string[] strings = File.ReadAllLines(path, Encoding.Unicode);
        List<string[]> raw_data = new List<string[]>();
        // на первом месте надписи
        for (int i = 5, count = 0; i < strings.Length; i += 2)
        {
            string[] item = new string[3];
            item[0] = strings[i + 0];
            item[1] = strings[i + 1];
            string answers = "";
            i += 2;
            if (i >= strings.Length)
                goto x;
            while (!string.IsNullOrEmpty(strings[i]))
            {
                answers += strings[i] + '\n';
                i++;
            }
        x:
            item[2] = answers;
            raw_data.Add(item);
            count++;
        }
        string[,] data = new string[raw_data.Count, 3];
        for (int i = 0; i < raw_data.Count; i++)
        {
            data[i, 0] = raw_data[i][0];
            data[i, 1] = raw_data[i][1];
            data[i, 2] = raw_data[i][2];
        }
        return data;
    }
}
