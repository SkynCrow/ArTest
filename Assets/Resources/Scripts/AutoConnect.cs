using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutoConnect : MonoBehaviour
{
    public UnityEvent host, connect;
    public InputField nombre;
    public string ip ;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        SetName();
#if UNITY_EDITOR
        host.Invoke();
#else
        GetComponent<MultiplayerMenu>().ipAddress.text = ip;
        connect.Invoke();
#endif
    }

    public void SetName()
    {
        PlayerInfo.Instance.PlayerName = nombre.text == null || nombre.text == "" ? "Player " + Random.Range(10, 100) : nombre.text;
        //PlayerPrefs.SetString("name",nombre.text == null || nombre.text == "" ? "Player "+Random.Range(10,100):nombre.text);
    }
}
