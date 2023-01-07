using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonController : MonoBehaviour, IDeselectHandler, ISelectHandler
{

    [SerializeField] Image img;
    [SerializeField] Color normalColor;
    [SerializeField] Color selectedColor;
    

    public void OnSelect(BaseEventData eventData)
    {
        img.color = selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        img.color = normalColor;
    }
}
