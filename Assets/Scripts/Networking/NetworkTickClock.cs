using Unity.Netcode;
using System;

public class NetworkTickClock : NetworkBehaviour
{
    static NetworkTickClock instance;
    int currentTick;
    int lastProcessedTick;

    public Action processTick;
    const int SYNC_PERIOD = 300;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this); 
        }
        currentTick = 0;
        if(IsClient){
            Synchronize();
        }
    }

    void Synchronize(){
        GetServerTickServerRPC(currentTick);
    }

    private void FixedUpdate() {
        // should be enough for thousands of hours
        currentTick++;  //= (currentTick + 1) % MAX_TICK;

        if(lastProcessedTick == -1) return;
        while(lastProcessedTick < currentTick){
            processTick();
            lastProcessedTick++;
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

    [ServerRpc]
    private void GetServerTickServerRPC(int clientTick){
        GetServerTickClientRPC(clientTick,currentTick);
    }

    [ClientRpc]
    private void GetServerTickClientRPC(int clientTick,int serverTick){
            
        currentTick = serverTick + (currentTick - clientTick)/2;
        if(lastProcessedTick == -1){
            lastProcessedTick = currentTick;
        }
    }
}

public interface ITickable{
    void ProcessTick();
}