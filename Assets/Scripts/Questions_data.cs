using System;
using System.Collections.Generic;

[Serializable]
public class Questions_data
{
    public int score;
    public int num;
    public string text;
    public List<string> answers;

    public Questions_data()
    {
        answers = new List<string>();
    }
}
