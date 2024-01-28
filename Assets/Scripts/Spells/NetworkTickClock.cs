using Unity.Netcode;
using System;
using UnityEngine;

public class NetworkTickClock : NetworkBehaviour
{
    public static NetworkTickClock instance;
    public int currentTick{get; private set;}
    int lastProcessedTick;

    public Action processTick;
    const int SYNC_PERIOD = 300;

    public override void OnNetworkSpawn()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this); 
        }
        currentTick = 0;
        if(!IsServer){
            Synchronize();
        }
    }

    void Synchronize(){
        GetServerTickServerRPC(OwnerClientId,currentTick,new ServerRpcParams());
    }

    private void FixedUpdate() {
        // should be enough for thousands of hours
        currentTick++;  //= (currentTick + 1) % MAX_TICK;

        if(lastProcessedTick == -1) return;
        while(lastProcessedTick < currentTick){
            lastProcessedTick++;
            processTick?.Invoke();
        }
        if(IsClient && currentTick % SYNC_PERIOD==0){
            Synchronize();
        }
    }

    public void RollForward(ITickable delayedObject,int startingTick){
        while(startingTick < currentTick){
            delayedObject.ProcessTick();
            startingTick++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetServerTickServerRPC(ulong clientId, int clientTick, ServerRpcParams serverRpcParams = default){
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{serverRpcParams.Receive.SenderClientId}
            }
        };
        GetServerTickClientRPC(clientTick,currentTick,clientRpcParams);
    }

    [ClientRpc]
    private void GetServerTickClientRPC(int clientTick,int serverTick,ClientRpcParams clientRpcParams = default){
            
        currentTick = serverTick + (currentTick - clientTick)/2;
        Debug.Log("Synchronisation:\n server " + serverTick.ToString() + "\n client " + clientTick.ToString() + "\n current " + currentTick.ToString());
        if(lastProcessedTick == -1){
            lastProcessedTick = currentTick;
        }
    }
}

public interface ITickable{
    void ProcessTick();
}