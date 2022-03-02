using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private float angularVelocity = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null){
            Vector3 targetPosition = player.transform.TransformPoint(offset);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            Quaternion targetRotation = player.transform.rotation;

            float delta = Quaternion.Angle(transform.rotation, targetRotation);

            if (delta > 0f) {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, smoothTime);
                t = 1.0f - (t / delta);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            }
        }
    }
}
