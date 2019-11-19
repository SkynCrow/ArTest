using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : GameLogicBehavior
{
    public static GameLogic Instance;
    public ControllerWithoutAR controller;
    public Dictionary<uint, int> scores;
    private bool start, finish;
    IClientMockPlayer mockPlayer;
    public uint id;
    List<IClientMockPlayer> currentPlayers;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        scores = new Dictionary<uint, int>();
        controller = GetComponent<ControllerWithoutAR>();
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (networkObject.IsServer)
        {
            Destroy(controller);
            networkObject.Time = 10;
        }
        mockPlayer = LobbyService.Instance.MyMockPlayer;
        GameGUI.INSTANCE.Player1Name.text = mockPlayer.Name;
        id = networkObject.MyPlayerId;
        Debug.Log("Usuario:" + id + " Nombre: " + mockPlayer.Name + " mockid: " + mockPlayer.NetworkId + " isOwner:" + networkObject.IsOwner);
        currentPlayers = LobbyService.Instance.MasterLobby.LobbyPlayers;
    }
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (networkObject == null)
            return;

        if (start && !finish)
        {
            if (!networkObject.IsServer)
                networkObject.SendRpc(RPC_SEND_SCORE, Receivers.All, id, controller.Points);
            int mm = (int)networkObject.Time / 60;
            int ss = (int)networkObject.Time - (mm * 60);
            GameGUI.INSTANCE.Time.text = mm.ToString("00") + ":" + ss.ToString("00");
            if (networkObject.IsServer)
            {

                if (networkObject.Time <= 0)
                {
                    networkObject.SendRpc(RPC_FINISH, Receivers.All);

                }
                networkObject.Time -= Time.deltaTime;
            }
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            networkObject.SendRpc(RPC_START, Receivers.All);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            networkObject.Time = 0;
        }
    }
    public override void Start(RpcArgs args)
    {
        Debug.Log("Start:" + args.Info.SendingPlayer.Name);
        if (!networkObject.IsServer)
            controller.enabled = true;
        start = true;
    }
    public override void Finish(RpcArgs args)
    {
        Debug.Log("Finish:" + id + " " + args.Info.SendingPlayer.NetworkId.ToString());
        GameGUI.INSTANCE.Time.text = "00:00";
        if (networkObject.IsServer)
        {

        }
        else
        {
            if (networkObject.IsOwner)
            {
                Debug.Log("holis" + id);
            }
            controller.enabled = false;
            string result = "";

            foreach (var item in scores)
            {
                result += item.Key + "\t:\t" + item.Value;
            }
            GameResult.Instance.result.text = result;
            GameResult.Instance.transform.GetChild(0).gameObject.SetActive(true);
        }
        finish = true;
    }

    public override void SendScore(RpcArgs args)
    {
        uint id = args.GetNext<uint>();
        int points = args.GetNext<int>();
        if (scores.ContainsKey(id))
        {
            scores[id] = points;
        }
        else
        {
            scores.Add(id, points);
        }
        if (id == this.id)
        {
            GameGUI.INSTANCE.Player1Score.text = points.ToString();
        }
        else
        {
            GameGUI.INSTANCE.Player2Score.text = points.ToString();
        }
        string scoresText = "";
        foreach (var item in scores)
        {
            scoresText += item.Key + "\t" + item.Value + "\n";
        }
        GameGUI.INSTANCE.devText.text = scoresText;
    }
}
