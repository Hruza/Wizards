using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabList", menuName = "PrefabList", order = 0)]
public class PrefabList : ScriptableObject {
    public List<GameObject> prefabs;
}