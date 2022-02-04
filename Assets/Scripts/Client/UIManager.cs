using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class UIManager : NetworkBehaviour
{
    public static UIManager Singleton;

    public Slider forcePercentSlider;
    public GameObject endPanel;
    public TMPro.TMP_Text winnderNickname;

    void Awake()
    {
        StartSingleton();
    }

    void Start()
    {
        BowController.LaunchForcePercentChanged += OnLaunchForcePercentChanged;
    }

    void OnLaunchForcePercentChanged(float forcePercent)
    {
        forcePercentSlider.value = forcePercent;
    }

    public void OnPlayAgain()
    {
        endPanel.SetActive(false);

        NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ShowEndGamePanel(string nickName)
    {
        if (!IsClient) return;

        endPanel.SetActive(true);
        winnderNickname.text = nickName;
    }

    private void StartSingleton()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
