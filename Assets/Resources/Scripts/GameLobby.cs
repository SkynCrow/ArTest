using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobby : GameLobbyBehavior
{
    public List<NetworkingPlayer> Players;
    public bool ImPlayer, ImEnemy;

    public override void SyncEnemy(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            string name = args.GetNext<string>();
            GameLobbyGUI.Instance.Player2Name.text = name;
            GameLobbyGUI.Instance.ColapseEnemy = true;
        }
    }

    public override void SyncPlayer(RpcArgs args)
    {
        if (networkObject.IsServer)
        {
            return;
        }
        string name = args.GetNext<string>();
        BMSLogger.Instance.LogFormat("Me llego un RCP de:{0} con el argumento:{1}",args.Info.SendingPlayer.NetworkId,name);
        if (networkObject.IsOwner)
        {
            GameLobbyGUI.Instance.Player1Name.text = name;
            GameLobbyGUI.Instance.ColapsePlayer = true;
        }
        else
        {
            GameLobbyGUI.Instance.Player2Name.text = name;
            GameLobbyGUI.Instance.ColapseEnemy = true;
        }
    }

    public override void SyncTime(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            float time = args.GetNext<float>();
            GameLobbyGUI.Instance.buttonImage.color = GameLobbyGUI.Instance.blue;
            GameLobbyGUI.Instance.TextButton.text = ((int)time).ToString();
        }
    }


    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (networkObject.IsOwner)
        {
            GameLobbyGUI.Instance.click += Instance_click;
            name = PlayerInfo.Instance.PlayerName;//"Player" + Random.Range(10, 100);
            networkObject.Owner.Name = name;
            networkObject.SendRpc(RPC_SYNC_PLAYER, Receivers.AllBuffered, name);
        }
    }
    private void Instance_click()
    {
        if (networkObject.IsOwner)
        {
            bool ready = networkObject.Ready;
            networkObject.Ready = ready;
            if (ready)
            {
                GameLobbyGUI.Instance.buttonImage.color = GameLobbyGUI.Instance.red;
            }
            else
            {
                GameLobbyGUI.Instance.buttonImage.color = GameLobbyGUI.Instance.green;
            }
        }
    }

}
