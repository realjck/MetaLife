using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetBackward;
    [SerializeField] private Vector3 offsetRightSide;
    [SerializeField] private Vector3 offsetLeftSide;
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private float angularVelocity = 0;
    public int direction;// 0 1 2 3 means N E S W ((NB ONLY N S here))
    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null){

            Vector3 o = offset;

            switch(direction){
                case 0: o = offset;
                break;

                case 1: o = offsetRightSide;
                break;

                case 2: o = offsetBackward;
                break;

                case 3: o = offsetLeftSide;
                break;
            }

            // move cam
            Vector3 targetPosition = player.transform.TransformPoint(o);
            // this is for smooth
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            // rotate cam
            // when backward, GET player rotation
            Quaternion targetRotation = player.transform.rotation;
            // else, calculate
            if (direction != 0){
                Vector3 lookPoint = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                Vector3 direction =  lookPoint - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            }            
            
            // this is for smooth
            float delta = Quaternion.Angle(transform.rotation, targetRotation);
            if (delta > 0f) {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, smoothTime);
                t = 1.0f - (t / delta);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            }
        }
    }
}
