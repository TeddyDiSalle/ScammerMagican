using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler,
    IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIChange();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIChange();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISelect();
    }
}
