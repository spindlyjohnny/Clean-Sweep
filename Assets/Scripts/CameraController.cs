using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lookAhead = 0;
    public float smoothing = 1f;
    public bool followtarget;
    public Vector3 targetposition;
    // Start is called before the first frame update
    void Start()
    {
        followtarget = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (followtarget) {
            // Calculates camera position
            targetposition = new Vector3(target.position.x + lookAhead * Mathf.Sign(target.localScale.x), target.position.y, transform.position.z);
            // lerps camera to player
            transform.position = Vector3.Lerp(transform.position, targetposition, Time.deltaTime * smoothing);
        }
    }
}
