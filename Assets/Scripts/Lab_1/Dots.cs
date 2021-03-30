using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform tooltip; 

    private void Awake()
    {
        tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(false);
    }

    public void Set_data(Vector2 position, Transform parent, int x, float y, Color color)
    {
        transform.GetComponent<Image>().color = color;
        transform.SetParent(parent, false);
        transform.GetComponent<RectTransform>().anchoredPosition = position;
        Vector2 size = new Vector2(11, 11);
        if (color == Color.white)
            size.Set(8, 8);
        transform.GetComponent<RectTransform>().sizeDelta = size;
        Text tooltip_text = tooltip.Find("Text").GetComponent<Text>();
        tooltip_text.text = x.ToString() + " : " + y.ToString("0.00");
        tooltip.Find("Background").GetComponent<RectTransform>().sizeDelta =
            new Vector2(tooltip_text.preferredWidth, tooltip_text.preferredHeight);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}
