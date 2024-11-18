using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pingPongCube : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        float pingPongValue = Mathf.PingPong(Time.time, 2f);
        float smoothValue = Mathf.Lerp(0f, 2f, Mathf.PingPong(Time.time * 0.5f, 1f));
        // `smoothValue`는 0부터 2까지 부드럽게 증가하고 감소합니다.
        transform.position = new Vector3(transform.position.x, smoothValue, transform.position.z);
    }
}

