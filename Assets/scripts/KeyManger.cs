using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManger : MonoBehaviour
{
    public bool KeyMangerSet;
    private Option owner;
    private int openedFrame;
    public void Begin(Option option, int index)
    {
        if (GameData.Instance == null)
            return;
        if (owner != null)
            owner.ClearKeySelection();
        owner = option;
        GameData.Instance.Num = index;
        openedFrame = Time.frameCount;
        KeyMangerSet = true;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!KeyMangerSet || Time.frameCount == openedFrame || GameData.Instance == null)
            return;
        if (GameData.Instance.DetectAndSetKey())
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        KeyMangerSet = false;
        if (owner != null)
            owner.ClearKeySelection();
        owner = null;
    }
}
