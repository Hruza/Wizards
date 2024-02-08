using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public Transform cam;

    public LayerMask layerMask;

    const float MAX_DISTANCE=200;
    // Start is called before the first frame update

    // Update is called once per frame  
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * 500, Color.yellow);
        if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask.value))
        {
            transform.position = hit.point;
            CrosshairController.SetDistance(hit.distance,1,MAX_DISTANCE);
        }
        else{
            transform.position = cam.position + cam.TransformDirection(Vector3.forward)*MAX_DISTANCE;
            CrosshairController.SetDistance(MAX_DISTANCE,1,MAX_DISTANCE);
        }
    }
}
