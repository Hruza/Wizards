using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkStartUI : MonoBehaviour
{
    public Button HostButton;
    public Button ClientButton;

    void Start(){
        HostButton.onClick.AddListener(StartHost);
        ClientButton.onClick.AddListener(StartClient);
    }

    void StartHost(){
        Debug.Log("Starting host");
        NetworkManager.Singleton.StartHost();
        HideUI();
    }

    void StartClient(){
        Debug.Log("Starting client");
        NetworkManager.Singleton.StartClient();
        HideUI();
    }

    void HideUI() => gameObject.SetActive(false);
}
