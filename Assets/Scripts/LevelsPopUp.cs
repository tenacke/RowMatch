using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelsPopUp : MonoBehaviour
{
    public GameObject MainPanel;
    private GameObject[] _levels;
    public static int CurrentLevel = 1;
    public static List<int> HighScores;

    public static string PlayerInfoName = "PlayerInfo";
    // Start is called before the first frame update
    void Start()
    {
        _levels = GameObject.FindGameObjectsWithTag("Level");
        
        // PlayerInfo playerInfo = new PlayerInfo();
        // HighScores.AddRange(new []{0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
        // playerInfo.HighScores = HighScores;
        // DataSaver.SaveData(playerInfo, PlayerInfoName);
        
        PlayerInfo playerInfo = DataSaver.LoadData<PlayerInfo>(PlayerInfoName);
        if (playerInfo == null) 
        {
            Debug.Log("PlayerInfo is null");
            playerInfo = new PlayerInfo();
            DataSaver.SaveData(playerInfo, PlayerInfoName);
        }
        HighScores = playerInfo.HighScores;
        int i;
        for (i = 0; i < HighScores.Count; i++)
        {
            if (HighScores[i] == 0)
            {
                break;
            }
            SetButtonActive(i+1);
            SetHighScore(i+1, HighScores[i]);
        }
        if (i < HighScores.Count)
        {
            SetButtonActive(i + 1);
            SetNoScore(i + 1);
        }
    }

    public void SetButtonActive(int level)
    {
        GameObject go = _levels[level-1];
        Button button = go.GetComponentInChildren<Button>();
        GameObject buttonGO = button.gameObject;
        button.interactable = true;
        Transform buttonTransform = buttonGO.GetComponent<Transform>();
        buttonTransform.Find("ButtonText").gameObject.SetActive(true);
        buttonTransform.Find("Image").gameObject.SetActive(false);
    }
    
    public void SetNoScore(int level)
    {
        GameObject go = _levels[level-1];
        Transform buttonTransform = go.GetComponent<Transform>();
        TextMeshProUGUI text = buttonTransform.Find("LevelHighScoreText").GetComponent<TextMeshProUGUI>();
        text.text = "No Score";
    }
    
    public void SetHighScore(int level, int score)
    {
        GameObject go = _levels[level-1];
        Transform buttonTransform = go.GetComponent<Transform>();
        TextMeshProUGUI text = buttonTransform.Find("LevelHighScoreText").GetComponent<TextMeshProUGUI>();
        text.text = $"High Score: {score.ToString()}";
    }

    public void LoadLevel(int level)
    {
        CurrentLevel = level;
        SceneManager.LoadScene(1);
        Debug.Log($"Loading level {level.ToString()}...");
    }

    public void LoadPopUp()
    {
        Transform popUpTransform = transform.parent.parent;
        popUpTransform.localPosition = new Vector3(0, 1400, 0);
        popUpTransform.DOLocalMoveY(0, 0.5f);
    }
    
    public void ClosePopUp()
    {
        Transform popUpTransform = transform.parent.parent;
        popUpTransform.DOLocalMoveY(1400, 0.5f).onKill = () =>
        {
            popUpTransform.gameObject.SetActive(false); 
            MainPanel.SetActive(true);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
