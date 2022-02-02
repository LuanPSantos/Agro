using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class StartNetworkManager : MonoBehaviour
{
    public TMP_InputField code;
    public TMP_Text status;

    public bool runLocal = true;

    void Start()
    {
        RelayManager.Singleton.CodeGenereted += CodeGeneretedHandle;
    }

    private void CodeGeneretedHandle(string joinCode)
    {
        status.SetText("Code to connect is " + joinCode);
    }

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
        if (RelayManager.Singleton.IsRelayEnabled && HasCode() && !runLocal)
        {
            await RelayManager.Singleton.JoinRelay(code.text);
        }
        NetworkManager.Singleton.StartClient();
    }

    private async void StartHost()
    {
        if (RelayManager.Singleton.IsRelayEnabled && !runLocal)
        {
            await RelayManager.Singleton.SetUpRelay();
        }

        NetworkManager.Singleton.StartHost();
    }

    private bool HasCode()
    {
        return !string.IsNullOrEmpty(code.text);
    }
}
