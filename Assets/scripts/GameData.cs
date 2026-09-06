using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveFileData
{
    public KeyCode[] GameManger_KeySet =
    {
        KeyCode.W,
        KeyCode.S,
        KeyCode.A,
        KeyCode.D
    };
}

[DefaultExecutionOrder(-100)]
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    private string filePath;
    public string[] LoadData;
    public bool Check;
    public int Num;
    public GameObject Options;
    public SaveFileData data = new SaveFileData();
    private Operaiton optionsScript;
    public bool Pick_NOHA;
    public bool Pick_ENHA;
    public bool Pick_LUCY;
    public GameObject Choose;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
        LoadGame();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        BindOptions();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindOptions();
    }

    void BindOptions()
    {
        if (Options == null)
        {
            foreach (Operaiton candidate in FindObjectsOfType<Operaiton>(true))
            {
                if (!candidate.CompareTag("Option"))
                    continue;
                Options = candidate.gameObject;
                break;
            }
        }

        if (Options == null)
            return;
        optionsScript = Options.GetComponent<Operaiton>();
        Options.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance != this)
            return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    void EnsureKeySettings()
    {
        if (data == null)
            data = new SaveFileData();
        if (data.GameManger_KeySet == null || data.GameManger_KeySet.Length != 4)
            data.GameManger_KeySet = new[]
            {
                KeyCode.W,
                KeyCode.S,
                KeyCode.A,
                KeyCode.D
            };
    }

    public void SaveGame()
    {
        EnsureKeySettings();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(filePath))
                data = JsonUtility.FromJson<SaveFileData>(File.ReadAllText(filePath));
        }
        catch (Exception exception)when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
        {
            Debug.LogWarning("Could not load key settings: " + exception.Message);
        }

        EnsureKeySettings();
    }

    public void ResetGame()
    {
        EnsureKeySettings();
        data.GameManger_KeySet[0] = KeyCode.W;
        data.GameManger_KeySet[1] = KeyCode.S;
        data.GameManger_KeySet[2] = KeyCode.A;
        data.GameManger_KeySet[3] = KeyCode.D;
    }

    public bool DetectAndSetKey()
    {
        EnsureKeySettings();
        if (Num < 0 || Num >= data.GameManger_KeySet.Length)
            return false;
        if (Input.anyKeyDown)
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    if (k < KeyCode.Mouse0)
                    {
                        data.GameManger_KeySet[Num] = k;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void Update()
    {
        if (Choose == null)
        {
            Choose = GameObject.FindWithTag("Choose");
        }

        if (Choose != null)
        {
            Pick_ENHA = Choose.GetComponent<Choose_Character>().ENHA;
            Pick_NOHA = Choose.GetComponent<Choose_Character>().NOHA;
            Pick_LUCY = Choose.GetComponent<Choose_Character>().LUCY;
        }
    }
}
