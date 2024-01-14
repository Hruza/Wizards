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
    [SerializeField]NetworkPrefabsList networkProjectiles;
    [SerializeField]NetworkPrefabsList networkEffects;
    PrefabIndexer projectileIndexer;

    public override void OnNetworkSpawn()
    {
        projectileIndexer = new PrefabIndexer(networkProjectiles);
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

    public void CreateProjectile(GameObject projectile,Vector3 pos, Vector3 velocity,float scale, float g){
        SpawnProjectileServerRPC(projectileIndexer.PrefabToId(projectile), pos, velocity,scale,g);
    }


    [ServerRpc]
    public void SpawnProjectileServerRPC(int prefabId, Vector3 position, Vector3 velocity,float scale, float g){
        Debug.Log("Creating prefab velocity "+velocity.ToString());
        Quaternion rotation = Quaternion.LookRotation(velocity,Vector3.up);

        NetworkObject proj = NetworkObjectPool.Singleton.GetNetworkObject(projectileIndexer.IdToPrefab(prefabId),position,rotation);

        Projectile projComponent = proj.GetComponent<Projectile>();
        if(projComponent){
            projComponent.collBehaviour = new ExplosionCollisionBehaviour();
            projComponent.scale = scale;
            projComponent.g = g;
            projComponent.initSpeed = velocity.magnitude;
            projComponent.Initialize();
        }
        proj.Spawn();
    }

}

public class PrefabIndexer{
    Dictionary<int,GameObject> _idToPrefab;
    Dictionary<GameObject,int> _prefabToId;

    public PrefabIndexer(NetworkPrefabsList prefabList){
        _idToPrefab = new Dictionary<int, GameObject>();
        _prefabToId = new Dictionary<GameObject, int>();
        for (int i = 0; i < prefabList.PrefabList.Count; i++)
        {
            _idToPrefab[i] = prefabList.PrefabList[i].Prefab; 
            _prefabToId[prefabList.PrefabList[i].Prefab] = i;
        }
    }
    public int PrefabToId(GameObject prefab){
        return _prefabToId[prefab];
    }

    public GameObject IdToPrefab(int prefabId){
        return _idToPrefab[prefabId];
    }
}