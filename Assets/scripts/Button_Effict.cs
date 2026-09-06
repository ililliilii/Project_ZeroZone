using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Effict : MonoBehaviour, IPointerEnterHandler
    , IPointerExitHandler
{
    public GameObject Effect;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Effect.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Effect.SetActive(false);
    }

    // Start is called before the first frame update
}
