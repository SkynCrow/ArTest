using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine.UI;
using BeardedManStudios.Forge.Logging;

public class Player : PlayerBehavior
{
    GameInit gameInit;
    public uint id;
    private bool finish;
    private bool start;
    public int score;
    private string enemyName;
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            GameGUI.INSTANCE.Player1Name.text = value;
            name = value;
        }
    }

    public string EnemyName
    {
        get => enemyName;
        set
        {
            GameGUI.INSTANCE.Player2Name.text = value;
            enemyName = value;
        }
    }
    protected override void NetworkStart()
    {
        base.NetworkStart();
        id = networkObject.NetworkId;

        if (networkObject.IsServer)
        {
            FindObjectOfType<GameInit>().players.Add(this);
        }
        else
        {
            if (networkObject.IsOwner)
            {
                gameInit = FindObjectOfType<GameInit>();
                name = PlayerInfo.Instance.PlayerName;
                networkObject.SendRpc(RPC_SYNC_NAME, Receivers.AllBuffered, networkObject.NetworkId, Name);
            }
        }
    }

    private void Update()
    {
        if (networkObject == null || networkObject.IsServer)
            return;
        if (start && !finish)
        {
            //SyncScore
            if (networkObject.IsOwner)
            {
                score = Rayo.Instance.Hits;
                networkObject.Score = score;
                GameGUI.INSTANCE.Player1Score.text = score.ToString();
                float time = gameInit.networkObject.Time;
                int mm = (int)time / 60;
                int ss = (int)time - (mm * 60);
                GameGUI.INSTANCE.Time.text = mm.ToString("00") + ":" + ss.ToString("00");
            }
            else
            {
                score = networkObject.Score;
                GameGUI.INSTANCE.Player2Score.text = score.ToString();
            }

        }
    }
    public override void GameStart(RpcArgs args)
    {
        if (networkObject.IsServer)
        {
            Debug.Log("inicio");
        }
        if (networkObject.IsOwner)
        {
            Rayo.Instance.enabled = true;
        }
        start = true;
    }
    public override void GameFinish(RpcArgs args)
    {
        GameGUI.INSTANCE.Time.text = "00:00";
        Rayo.Instance.enabled = false;
        string result = "";
        foreach (var item in FindObjectsOfType<Player>())
        {
            result += item.name + "  :  " + item.networkObject.Score + "\n";
        }
        GameResult.Instance.result.text = result;
        GameResult.Instance.transform.GetChild(0).gameObject.SetActive(true);
        finish = true;
    }

    public override void SyncTime(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            float time = args.GetNext<float>();
            int mm = (int)time / 60;
            int ss = (int)time - (mm * 60);
            GameGUI.INSTANCE.Time.text = mm.ToString("00") + ":" + ss.ToString("00");
        }
    }

    public override void SyncEnemy(RpcArgs args)
    {
        string name = args.GetNext<string>();
        BMSLogger.Instance.LogFormat("SyncEnemy Me llego un RCP de:{0} con el argumento:{1}", args.Info.SendingPlayer.NetworkId, Name);
        if (!networkObject.IsOwner)
            return;
        EnemyName = name;
    }

    public override void SyncName(RpcArgs args)
    {
        uint argId = args.GetNext<uint>();
        string n = args.GetNext<string>();
        BMSLogger.Instance.LogFormat("SyncName {0} Me llego un RCP de:{1} con el argumento:{2}",networkObject.NetworkId, argId, n);
        if (networkObject.IsOwner)
        {
            Name = n;
        }
        else
        {
            name = n;
            EnemyName = n;
        }
    }
}
