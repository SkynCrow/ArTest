using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    public static GameResult Instance;
    public Text result;
    private void Awake()
    {
        Instance = this;
    }
    public void GoLobby()
    {
        SceneManager.LoadScene(1);
    }
}
