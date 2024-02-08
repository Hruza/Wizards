using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField]RectTransform center;
    [SerializeField]AnimationCurve distanceCurve;
    [SerializeField]float damping = 0.8f;
    static CrosshairController instance;
    private float currSize = 10;

    private void Start() {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }  

    private void SetSize(float size){
        currSize = Mathf.Lerp(currSize,size,damping);
        center.sizeDelta = new Vector2(Mathf.RoundToInt(currSize),Mathf.RoundToInt(currSize));
    }

    static public void SetDistance(float dist, float min=0, float max=100){
        instance.SetInstanceDistance(dist, min, max);
    }

    private void SetInstanceDistance(float dist, float min=0, float max=100){
        dist = (Mathf.Clamp(dist,min,max)-min)/(max-min);
        SetSize(distanceCurve.Evaluate(dist));
    }
}
