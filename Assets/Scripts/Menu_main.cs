using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_main : MonoBehaviour
{
    private void Awake()
    {
        Transform main = transform.Find("Main");
        main.Find("Lab_1").GetComponent<Button>().onClick.AddListener(Lab_1);
        main.Find("Lab_2").GetComponent<Button>().onClick.AddListener(Lab_2);
        main.Find("Quit").GetComponent<Button>().onClick.AddListener(Quit);
    }

    private void Lab_1()
    {
        SceneManager.LoadScene("Lab_1");
    }

    private void Lab_2()
    {
        SceneManager.LoadScene("Lab_2");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
