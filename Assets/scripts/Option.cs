using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool Save, Reset, Exit, IsExit, IsOption;
    public bool ForWard, Back, Right, Left;
    public bool FW, BC, RT, LT;
    public GameObject Checking;
    public GameObject KeyManger;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Save)
        {
            GameData.Instance.SaveGame();
        }

        if (Reset)
        {
            GameData.Instance.ResetGame();
            GameData.Instance.SaveGame();
        }

        if (Exit)
        {
            StartCoroutine(Set());
        }

        if (ForWard)
        {
            FW = true;
            KeyManger.GetComponent<KeyManger>().Begin(this, 0);
        }

        if (Back)
        {
            BC = true;
            KeyManger.GetComponent<KeyManger>().Begin(this, 1);
        }

        if (Right)
        {
            RT = true;
            KeyManger.GetComponent<KeyManger>().Begin(this, 3);
        }

        if (Left)
        {
            LT = true;
            KeyManger.GetComponent<KeyManger>().Begin(this, 2);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Save)
        {
            Checking.SetActive(true);
        }

        if (Reset)
        {
            Checking.SetActive(true);
        }

        if (Exit)
        {
            Checking.SetActive(true);
        }

        if (ForWard)
        {
            Checking.SetActive(true);
        }

        if (Back)
        {
            Checking.SetActive(true);
        }

        if (Right)
        {
            Checking.SetActive(true);
        }

        if (Left)
        {
            Checking.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Save)
        {
            Checking.SetActive(false);
        }

        if (Reset)
        {
            Checking.SetActive(false);
        }

        if (Exit)
        {
            Checking.SetActive(false);
        }

        if (ForWard)
        {
            Checking.SetActive(false);
        }

        if (Back)
        {
            Checking.SetActive(false);
        }

        if (Right)
        {
            Checking.SetActive(false);
        }

        if (Left)
        {
            Checking.SetActive(false);
        }
    }

    void Update()
    {
        if (IsOption)
        {
            IsExit = Checking.GetComponent<Option>().IsExit;
        }

        if (IsExit && IsOption)
        {
            StartCoroutine(Setting());
        }
    }

    IEnumerator Set()
    {
        IsExit = true;
        yield return new WaitForSeconds(0.1f);
        IsExit = false;
    }

    IEnumerator Setting()
    {
        yield return new WaitForSeconds(0.15f);
        gameObject.SetActive(false);
    }

    public void ClearKeySelection()
    {
        if (FW)
        {
            FW = false;
        }

        if (BC)
        {
            BC = false;
        }

        if (LT)
        {
            LT = false;
        }

        if (RT)
        {
            RT = false;
        }
    }
}
