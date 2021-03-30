using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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

    public static void Save_table_lab_2(string[,] data, string path)
    {
        string str = "";
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < 5; j++)
                str += data[i, j] + " ";
            str += '\n';
        }
        str = str.Remove(str.Length - 2); // удаление пустой последней строки и последнего пробела на предпоследней
        File.WriteAllText(path, str);
    }

    public static string[,] Load_table_lab_2(string path)
    {
        string[] strings = File.ReadAllLines(path);
        string[,] data = new string[strings.Length, 5];

        for (int i = 0; i < strings.Length; i++)
        {
            if (i == strings.Length - 1)
                strings[i] += ' ';
            string[] str = strings[i].Split(' ');
            if (str.Length - 1 != 5) // последний элемент это пустое место
                return null;
            for (int j = 0; j < str.Length - 1; j++)
                data[i, j] = str[j];                
        }
        return data;
    }
}
