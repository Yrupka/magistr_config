using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Calculation_formulas
{
    private struct triplet
    {
        public float item_1;
        public float item_2;
        public float item_3;

        public triplet(float item_1, float item_2, float item_3)
        {
            this.item_1 = item_1;
            this.item_2 = item_2;
            this.item_3 = item_3;
        }
    }
    // функция вычисляющая интерполирующую состовляющую графика, label_x,y - значения исходной функции
    private static float Interpolate(float x, List<float> label_x, List<float> label_y)
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
    public static List<float> Interpolated_x(List<float> x, int interpolation)
    {
        List<float> interpolated_x = new List<float>();

        float dot_place_procent = 1f / (interpolation + 1);

        for (int j = 0; j < x.Count - 1; j++)
            if (x[j] != x[j + 1])
                for (int i = 0; i <= interpolation; i++)
                    interpolated_x.Add(Mathf.Lerp(x[j], x[j + 1], i * dot_place_procent));
            else
                interpolated_x.Add(x[j]);

        interpolated_x.Add(x[x.Count - 1]);

        return interpolated_x;
    }

    // возвращает координаты точек по y для переданных значений х
    public static List<float> Interpolated_y(List<float> x, List<float> y, List<float> x_for_calculate)
    {
        List<float> interpolated_y = new List<float>();
        if (x.Count == 1)
        {
            interpolated_y.Add(y[0]);
            return interpolated_y;
        }
        // при ситуации когда будут повторы в оборота
        for (int i = 0; i < x_for_calculate.Count - 1; i++)
        {
            if (x_for_calculate[i] == x_for_calculate[i + 1])
                interpolated_y.Add(y[i]);
            else
                interpolated_y.Add(Interpolate(x_for_calculate[i], x, y));
        }
        int count = x_for_calculate.Count - 1;
        if (x_for_calculate[count] == x_for_calculate[count - 1])
            interpolated_y.Add(y[count]);
        else
            interpolated_y.Add(Interpolate(x_for_calculate[count], x, y));

        return interpolated_y;
    }

    // удаляет из массива дубликаты, интреполирует, возвращает дубликаты на свои места в результирующий массив
    public static List<float> Interpolate_dublicate(List<float> x, List<float> y, List<float> x_for_calculate)
    {
        List<float> x_tmp = new List<float>(x);
        List<float> y_tmp = new List<float>(y);
        // сохранить значения для дублирующих значений оборотов
        List<float> deleted_dublicate = new List<float>();
        for (int i = 0; i < x_tmp.Count - 1; i++)
        {
            if (x_tmp[i] == x_tmp[i + 1])
            {
                // сохраняем удаленные значения
                deleted_dublicate.Add(y_tmp[i + 1]);
                // удаляем значение из массивов
                y_tmp.RemoveAt(i + 1);
                x_tmp.RemoveAt(i + 1);
                i--;
            }
        }
        // сохранить их индексы, где они находились ранее
        List<int> deleted_index = new List<int>();
        for (int i = 0; i < x_for_calculate.Count - 1; i++)
            if (x_for_calculate[i] == x_for_calculate[i + 1])
                deleted_index.Add(i + 1); // сохраняем индекс

        List<float> interpolated_y = Interpolated_y(x_tmp, y_tmp, x_for_calculate.Distinct().ToList());

        // возвращение повторяющихся значений обратно на свои места
        for (int i = 0; i < deleted_index.Count; i++)
        {
            interpolated_y.Insert(deleted_index[i], deleted_dublicate[i]);
        }

        return interpolated_y;
    }

    public static (List<float>, List<float>, List<float>) interpolate_3d(
        List<float> x, List<float> y, List<float> z, int interpolation)
    {
        List<triplet> rpms = new List<triplet>();
        for (int i = 0; i < x.Count; i++)
            rpms.Add(new triplet(x[i], y[i], z[i]));

        var groups = rpms.GroupBy(a => a.item_1);
        List<List<float>> arr_1 = new List<List<float>>();
        List<List<float>> arr_2 = new List<List<float>>();
        List<float> arr_3 = new List<float>();
        foreach (var item in groups)
        {
            List<float> value_1 = new List<float>();
            List<float> value_2 = new List<float>();
            foreach (var it in item)
            {
                value_1.Add(it.item_2);
                value_2.Add(it.item_3);
            }
            arr_1.Add(value_1);
            arr_2.Add(value_2);
            arr_3.Add(item.ElementAt(0).item_1);
        }

        // нужно посчитать сколько точек в каждой отдельной группе оборотов
        List<float> interpolated_z = new List<float>();
        List<float> interpolated_y = new List<float>();
        List<float> interpolated_x = new List<float>();
        for (int i = 0; i < arr_1.Count; i++)
        {
            List<float> calculated_z = Calculation_formulas.Interpolated_x(arr_2[i], interpolation);
            List<float> calculated_y = Calculation_formulas.Interpolate_dublicate(arr_2[i], arr_1[i], calculated_z);
            // костыль, иногда пропускает значения программа, забить их 0, это только для 3д графика, на вычисления не влияет
            if (calculated_y.Count < calculated_z.Count)
            {
                int cc = calculated_z.Count - calculated_y.Count;
                for (int k = 0; k < cc; k++)
                {
                    calculated_y.Add(0);
                }
            }
                
            for (int j = 0; j < calculated_z.Count; j++)
                interpolated_x.Add(arr_3[i]);
            interpolated_y.AddRange(calculated_y);
            interpolated_z.AddRange(calculated_z);
        }
        
        return (interpolated_x, interpolated_y, interpolated_z);
    }

    // функция для сортировки 3 массивов, используя первый массив как ключ
    public static (List<float>, List<float>, List<float>) Sorting(
        List<float> a, List<float> b, List<float> c)
    {
        List<triplet> store = new List<triplet>();
        for (int i = 0; i < a.Count; i++)
            store.Add(new triplet(a[i], b[i], c[i]));

        store.Sort((first, second) => first.item_1.CompareTo(second.item_1));

        List<float> item_1 = new List<float>();
        List<float> item_2 = new List<float>();
        List<float> item_3 = new List<float>();

        foreach (triplet item in store)
        {
            item_1.Add(item.item_1);
            item_2.Add(item.item_2);
            item_3.Add(item.item_3);
        }

        return (item_1, item_2, item_3);
    }
}
