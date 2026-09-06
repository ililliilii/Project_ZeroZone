using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCodeTXT : MonoBehaviour
{
    public string FW,BC,LT,RT;
    public Text[] Txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StringKEYCODE();
        Txt[0].text = (FW);
        Txt[1].text = (BC);
        Txt[2].text = (LT);
        Txt[3].text = (RT);
    }
    void StringKEYCODE()
    {
        FW = GameData.Instance.data.GameManger_KeySet[0].ToString();
        BC = GameData.Instance.data.GameManger_KeySet[1].ToString();
        LT = GameData.Instance.data.GameManger_KeySet[2].ToString();
        RT = GameData.Instance.data.GameManger_KeySet[3].ToString();
    }
}
