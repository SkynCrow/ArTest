using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    public Text Time, Player1Name, Player2Name, Player1Score, Player2Score,devText;

    public static GameGUI INSTANCE;

    private void Awake()
    {
        INSTANCE = this;
    }
}
