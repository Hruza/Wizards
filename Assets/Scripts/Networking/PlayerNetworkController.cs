using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkController : NetworkBehaviour {
    [SerializeField] CinemachineVirtualCamera playerCamera; 
    [SerializeField] AudioListener audioListener;

    public override void OnNetworkSpawn(){
        if(!IsOwner){
            audioListener.enabled =false;
            playerCamera.Priority = 0;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            return;
        }

        audioListener.enabled = true;
        playerCamera.Priority = 100;

    }
}
