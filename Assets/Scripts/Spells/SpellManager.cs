using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class SpellManager : NetworkBehaviour
{
    private InputAction click;

    public Transform aimTarget;
    public GameObject projectile;
    public Transform projectileSource;
    public float projectileVelocity = 30f;

    public float g = 9.81f;
    public float maxDist = 100f;
    // Start is called before the first frame update
    Spell currentSpell;

    [SerializeField]Attack defaultAttack;
    [SerializeField]SpellModifier defaultModifier;
    
    public ElementDefinition element;
    InputAction fire;
    [SerializeField]PrefabList networkProjectiles;
    [SerializeField]NetworkPrefabsList networkEffects;
    PrefabIndexer projectileIndexer;

    public override void OnNetworkSpawn()
    {
        projectileIndexer = new PrefabIndexer(networkProjectiles.prefabs);
        if(!IsOwner) return;
        Debug.Log(defaultModifier);
        currentSpell = new Spell(element,defaultAttack,defaultModifier,this);

        fire = GetComponent<PlayerInput>().actions.FindAction("Player/Attack");

        fire.started +=
            _ => OnStartAttack();

        fire.canceled +=
            _ => OnEndAttack();

    }

    void OnStartAttack(){
        currentSpell.attack.OnStartAttack(aimTarget,projectileSource);
    }

    void OnDoAttack(){
        currentSpell.attack.OnDoAttack(aimTarget,projectileSource);
    }

    void OnEndAttack(){
        currentSpell.attack.OnEndAttack(aimTarget,projectileSource);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        if(fire.enabled && fire.IsPressed()) OnDoAttack();
    }

    int prefabId=0;

    public void CreateProjectile(GameObject projectile,Vector3 pos, Vector3 velocity,float scale, float g){
        prefabId = projectileIndexer.PrefabToId(projectile);
        SpawnProjectileServerRPC(prefabId, NetworkTickClock.instance.currentTick, pos, velocity,scale,g);
        InstantiateProjectile(prefabId, -1, pos, velocity, scale, g);
    }


    [ServerRpc]
    public void SpawnProjectileServerRPC(int prefabId, int tick, Vector3 position, Vector3 velocity,float scale, float g){
        InsantiateProjectileClientRPC(prefabId, tick, position, velocity, scale, g);   
    }

    [ClientRpc]
    public void InsantiateProjectileClientRPC(int prefabId, int tick, Vector3 position, Vector3 velocity,float scale, float g){
        if(!IsOwner){
            InstantiateProjectile(prefabId,tick,position,velocity,scale,g);
        }
    }

    public void InstantiateProjectile(int prefabId, int tick, Vector3 position, Vector3 velocity,float scale, float g){
        Debug.Log("Creating prefab velocity "+velocity.ToString());
        Quaternion rotation = Quaternion.LookRotation(velocity,Vector3.up);

        GameObject proj = Instantiate(projectileIndexer.IdToPrefab(prefabId),position,rotation);

        IProjectile projComponent = proj.GetComponent<IProjectile>();
        if(projComponent != null){
            projComponent.Initialize(new ExplosionCollisionBehaviour(),scale,g,velocity.magnitude);
        }
        if(tick >=0){
            NetworkTickClock.instance.RollForward(projComponent,tick);
        }
    }
}

public class PrefabIndexer{
    Dictionary<int,GameObject> _idToPrefab;
    Dictionary<GameObject,int> _prefabToId;

    public PrefabIndexer(List<GameObject> prefabList){
        _idToPrefab = new Dictionary<int, GameObject>();
        _prefabToId = new Dictionary<GameObject, int>();
        for (int i = 0; i < prefabList.Count; i++)
        {
            _idToPrefab[i] = prefabList[i]; 
            _prefabToId[prefabList[i]] = i;
        }
    }
    public int PrefabToId(GameObject prefab){
        return _prefabToId[prefab];
    }

    public GameObject IdToPrefab(int prefabId){
        return _idToPrefab[prefabId];
    }
}
