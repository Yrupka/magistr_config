using System.Collections.Generic;
using UnityEngine;

public static class Calculation_formulas
{
    // функция вычисляющая интерполирующую состовляющую графика, label_x,y - значения исходной функции
    private static float Interpolate(float x, List<int> label_x, List<float> label_y)
    {
        float answ = 0f;
        for (int j = 0; j < label_x.Count; j++)
        {
            float l_j = 1f;
            for (int i = 0; i < label_x.Count; i++)
            {
                if (i == j)
                    l_j *= 1f;
                else
                    l_j *= (x - label_x[i]) / (label_x[j] - label_x[i]);
            }
            answ += l_j * label_y[j];
        }
        return answ;
    }

    // возвращает новые координаты точек по x с учетом интерполированных значений
    public static List<int> Interpolated_x(List<int> x, int interpolation)
    {
        List<int> interpolated_x = new List<int>();

        float dot_place_procent = 1f / (interpolation + 1);

        for (int j = 0; j < x.Count - 1; j++)
            for (int i = 0; i <= interpolation; i++)
                interpolated_x.Add((int)Mathf.Lerp(x[j], x[j + 1], i * dot_place_procent));

        interpolated_x.Add(x[x.Count - 1]);

        return interpolated_x;
    }

    // возвращает координаты точек по y для переданных значений х
    public static List<float> Interpolated_y(List<int> x, List<float> y, List<int> x_for_calculate)
    {
        List<float> interpolated_y = new List<float>();
        for (int i = 0; i < x_for_calculate.Count; i++)
            interpolated_y.Add(Interpolate(x_for_calculate[i], x, y));
        return interpolated_y;
    }
}
