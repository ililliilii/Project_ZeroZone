using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool RTG, MainMenu, Options, QTD, Check;
    public GameObject Checking;
    public GameObject OptionKey, Menu;
    void Awake()
    {
        if (OptionKey == null && GameData.Instance != null)
            OptionKey = GameData.Instance.Options;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RTG)
        {
            Menu.SetActive(false);
        }

        if (MainMenu)
        {
            SceneManager.LoadScene("MainScene");
        }

        if (Options && !Check)
        {
            if (OptionKey == null && GameData.Instance != null)
                OptionKey = GameData.Instance.Options;
            if (OptionKey == null)
                return;
            Check = true;
            OptionKey.SetActive(true);
        }

        if (QTD)
        {
            Application.Quit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (RTG)
        {
            Check = false;
            Checking.SetActive(true);
        }

        if (MainMenu)
        {
            Check = false;
            Checking.SetActive(true);
        }

        if (Options)
        {
            Check = false;
            GameData.Instance.LoadGame();
            Checking.SetActive(true);
        }

        if (QTD)
        {
            Check = false;
            Checking.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (RTG)
        {
            Check = true;
            Checking.SetActive(false);
        }

        if (MainMenu)
        {
            Check = true;
            Checking.SetActive(false);
        }

        if (Options)
        {
            Check = true;
            Checking.SetActive(false);
        }

        if (QTD)
        {
            Check = true;
            Checking.SetActive(false);
        }
    }

    void Update()
    {
        if (OptionKey == null && GameData.Instance != null)
            OptionKey = GameData.Instance.Options;
        if (OptionKey == null)
        {
            return;
        }
    }
}
