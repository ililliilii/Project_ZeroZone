using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Choose_Character : MonoBehaviour
{
    public GameObject[] Character;
    public bool NOHA;
    public bool ENHA;
    public bool LUCY;
    private bool Ready;
    public Image Start_Image;
    private void Update()
    {
        if (!Ready)
        {
            Start_Image.color = new Color32(105, 105, 105, 255);
        }

        if (Ready)
        {
            Start_Image.color = new Color32(218, 218, 218, 255);
        }
    }

    public void Gamestart()
    {
        if (Ready)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void SelectENHA()
    {
        if (!Ready)
        {
            ResetSelections();
            ENHA = true;
            Character[0].SetActive(true);
        }
    }

    public void SelectNOHA()
    {
        if (!Ready)
        {
            ResetSelections();
            NOHA = true;
            Character[1].SetActive(true);
        }
    }

    public void SelectLUCY()
    {
        if (!Ready)
        {
            ResetSelections();
            LUCY = true;
            Character[2].SetActive(true);
        }
    }

    public void Select()
    {
        if ((NOHA || ENHA || LUCY) && !Ready)
        {
            Ready = true;
        }
        else if (Ready)
        {
            Ready = false;
        }
    }

    private void ResetSelections()
    {
        ENHA = false;
        NOHA = false;
        LUCY = false;
        Character[0].SetActive(false);
        Character[1].SetActive(false);
        Character[2].SetActive(false);
    }
}
