using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AINetworkController : NetworkBehaviour
{
    void Start(){
        Initilise();
    }
    public override void OnNetworkSpawn(){
        Initilise();
    }

    private void Initilise(){
        if(!IsServer){
            GetComponent<AiAgent>().enabled = false;
            return;
        }
    }
}

