using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainScene_UI : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public bool Single,OP,Exit,Check;
    public GameObject Checking;
    public GameObject OptionKey;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Single)
        {
            SceneManager.LoadScene("UI");
        }
        if (OP&&!Check)
        {
            Check = true;
            OptionKey.SetActive(true);
        }
        if (Exit)
        {
            Application.Quit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Single)
        {
            Check = false;
            Checking.SetActive(true);

        }
        if (OP)
        {
            Check = false;
            GameData.Instance.LoadGame();
            Checking.SetActive(true);

        }
        if (Exit)
        {
            Check = false;
            Checking.SetActive(true);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(Single)
        {
            Check = true;
            Checking.SetActive(false);
        }
        if (OP)
        {
            Check = true;
            Checking.SetActive(false);
        }
        if (Exit)
        {
            Check = true;
            Checking.SetActive(false);
        }
    }
}