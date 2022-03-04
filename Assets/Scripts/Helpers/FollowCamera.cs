using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetBackward;
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private float angularVelocity = 0;
    public bool isBackward;

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null){

            Vector3 o = offset;
            if (isBackward){
                o = offsetBackward;
            }
            Vector3 targetPosition = player.transform.TransformPoint(o);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            Quaternion targetRotation = player.transform.rotation;
            if (isBackward){
                Vector3 lookPoint = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                Vector3 direction =  lookPoint - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            }            
            
            float delta = Quaternion.Angle(transform.rotation, targetRotation);

            if (delta > 0f) {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, smoothTime);
                t = 1.0f - (t / delta);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            }
        }
    }
}
