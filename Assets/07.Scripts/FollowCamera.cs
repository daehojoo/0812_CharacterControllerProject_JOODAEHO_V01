using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform targetTr;
    [SerializeField] private Transform cameraTr;
    [SerializeField] private float height;
    [SerializeField] private float distance;
    [SerializeField] private float targetOffset = 1f;
    [SerializeField] private float moveDamping = 5f;
    [SerializeField] private float rotDamping = 10f;

    void Start()
    {
        cameraTr = transform;
        
    }
    void LateUpdate()
    {
        var camPos = targetTr.position - (Vector3.forward * distance) + (Vector3.up * height);
        cameraTr.position = Vector3.Slerp(cameraTr.position,camPos,moveDamping * Time.deltaTime);

        cameraTr.rotation = Quaternion.Slerp(cameraTr.rotation, targetTr.rotation, rotDamping * Time.deltaTime);
        cameraTr.LookAt(targetTr.position + (Vector3.up * targetOffset));



    }
}
