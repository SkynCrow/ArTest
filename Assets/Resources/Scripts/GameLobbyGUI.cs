using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyGUI : MonoBehaviour
{
    public Color green, red, blue;
    public RectTransform Player1Panel, Player2Panel;
    public Text Player1Name, Player2Name, TextButton;
    public Button button;
    public Image buttonImage;
    public bool ColapsePlayer, ColapseEnemy;
    public float tPlayer, tEnemy, speedFactor = 1;
    public static GameLobbyGUI Instance;
    public delegate void Click();
    public event Click click;
    public void Awake()
    {
        Instance = this;
    }
   
    // Update is called once per frame
    void Update()
    {
        tPlayer = Mathf.Clamp01(tPlayer + (speedFactor * Time.deltaTime * (ColapsePlayer ? 1 : -1)));
        Player1Panel.anchoredPosition = new Vector2(Mathf.Lerp(-Screen.width, 0, tPlayer), Player1Panel.anchoredPosition.y);
        tEnemy = Mathf.Clamp01(tEnemy + (speedFactor * Time.deltaTime * (ColapseEnemy ? 1 : -1)));
        Player2Panel.anchoredPosition = new Vector2(Mathf.Lerp(Screen.width, 0, tEnemy), Player2Panel.anchoredPosition.y);
    }
    public void SetReady()
    {
        click?.Invoke();
        foreach (var item in FindObjectsOfType<GameLobby>())
        {
            if (item.networkObject.IsOwner)
            {
                item.networkObject.Ready = true;
            }
        }
    }
}
