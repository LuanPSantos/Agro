using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkStartManager : MonoBehaviour
{
    public TMP_InputField code;

    public void StartHostOrClient()
    {
        if (HasCode())
        {
            StartClient();   
        }
        else
        {
            StartHost();
        }        
    }

    private async void StartClient()
    {
        if (RelayManager.Singleton.IsRelayEnabled && HasCode())
        {
            await RelayManager.Singleton.JoinRelay(code.text);
        }
        NetworkManager.Singleton.StartClient();
    }

    private async void StartHost()
    {
        if (RelayManager.Singleton.IsRelayEnabled)
        {
            await RelayManager.Singleton.SetUpRelay();
        }

        NetworkManager.Singleton.StartHost();
    }

    private bool HasCode()
    {
        Debug.Log(code.text);
        return !string.IsNullOrEmpty(code.text);
    }
}
