using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : GameInitBehavior
{
    public GameObject[] gameObjectsAndroid, gameObjectsWindows;
    private bool start;
    // Start is called before the first frame update
    private float gameTime = 120;
    public List<Player> players;
    private bool finish;

    public float GameTime
    {
        get
        {
            return gameTime;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            gameTime = value;
        }
    }
    private void Awake()
    {
#if UNITY_ANDROID
       bool android = true;
#else
       bool android = false;
#endif
        foreach (var item in gameObjectsAndroid)
        {
            item.gameObject.SetActive(android);
        }
        foreach (var item in gameObjectsWindows)
        {
            item.gameObject.SetActive(!android);
        }
    }
    private IEnumerator Start()
    {

        yield return new WaitUntil(() => networkObject != null);
        yield return new WaitForSeconds(3);
        if (networkObject.IsServer)
        {
            players.ForEach((x) => x.networkObject.SendRpc(Player.RPC_GAME_START, Receivers.AllBuffered));
            start = true;
        }
        yield return null;
    }
    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (networkObject.IsServer)
        {
            players = new List<Player>();
            networkObject.Time = GameTime;
        }
        else
        {
            NetworkManager.Instance.InstantiatePlayer();
        }
    }

    private void Update()
    {
        if (networkObject == null)
            return;
        if (!start)
            return;
        if (networkObject.IsServer)
        {
            GameTime -= Time.deltaTime;
            networkObject.Time = GameTime;
            players.ForEach((x) => x.networkObject.SendRpc(Player.RPC_SYNC_TIME, Receivers.Owner, gameTime));
            players.ForEach(x =>
            {
                if (x.networkObject.Score >= 8)
                {
                    finish = true;
                }
            });
            if (GameTime <= 0 || finish)
            {
                players.ForEach((x) => x.networkObject.SendRpc(Player.RPC_GAME_FINISH, Receivers.AllBuffered));
                start = false;
                StartCoroutine(ReturnToLobby());
            }
            
        }
        
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }
}
