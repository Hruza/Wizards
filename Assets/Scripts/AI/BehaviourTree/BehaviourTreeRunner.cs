using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // Start is called before the first frame update
    void Start()
    {
        tree = tree.Clone();
        AiAgent aiAgent = GetComponent<AiAgent>();
        if(aiAgent.enabled) tree.Bind(aiAgent);
        else enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
