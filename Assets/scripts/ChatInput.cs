using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, null, "ChatHandler")]
public class ChatInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public RectTransform contentRect;
    public GameObject Chat;
    private GameObject textPrefab;
    private void Start()
    {
        textPrefab = Chat;
        BindInputField();
    }

    void BindInputField()
    {
        inputField.onSubmit.AddListener((inputText) =>
        {
            if (string.IsNullOrWhiteSpace(inputText))
                return;
            GameObject instGo = GameObject.Instantiate(textPrefab, contentRect, false);
            TMP_Text text = instGo.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = inputText;
            }

            inputField.text = "";
            inputField.ActivateInputField();
        });
    }
}
