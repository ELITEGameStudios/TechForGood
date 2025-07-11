using UnityEngine;
using System;

public class RealTime : MonoBehaviour
{
    DateTime time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time = System.DateTime.UtcNow.ToLocalTime();
        Debug.Log(time);
    }
}
