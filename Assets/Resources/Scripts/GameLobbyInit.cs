using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using UnityEngine.SceneManagement;

public class GameLobbyInit : MonoBehaviour
{
    public static GameLobbyInit Instance;
    public List<GameLobby> users;
    public float t = 5;

    private void Awake()
    {
        Instance = this;
        users = new List<GameLobby>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => NetworkManager.Instance != null);
        if (NetworkManager.Instance.IsServer)
        {
            NetworkManager.Instance.Networker.playerConnected += Networker_playerConnected;
            NetworkManager.Instance.objectInitialized += Instance_objectInitialized;
        }
        else
        {
            NetworkManager.Instance.InstantiateGameLobby();
        }
    }

    private void OnDisable()
    {
        NetworkManager.Instance.objectInitialized -= Instance_objectInitialized;
    }

    private void Instance_objectInitialized(INetworkBehavior unityGameObject, NetworkObject obj)
    {
        BMSLogger.Instance.Log("Instanciado:" + (unityGameObject as GameLobby).networkObject.NetworkId);
        if (unityGameObject is GameLobby)
        {
            users.Add(unityGameObject as GameLobby);
        }
    }

    private void Networker_playerConnected(NetworkingPlayer player, NetWorker sender)
    {
        if (users.Count == 2)
        {
            sender.Disconnect(true);
        }
    }

    private void Update()
    {
        if (NetworkManager.Instance == null)
            return;
        if (!NetworkManager.Instance.Networker.IsServer)
            return;
        if (users.Count != 2)
            return;
        foreach (var item in users)
        {
            if (!item.networkObject.Ready)
            {
                t = 5;
                return;
            }
        }
        t -= Time.deltaTime;
        users.ForEach((x) => x.networkObject.SendRpc(GameLobby.RPC_SYNC_TIME, Receivers.All, t));
        if (t <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }
}
