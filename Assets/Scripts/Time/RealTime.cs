using UnityEngine;
using System;
using UnityEngine.Rendering;

public class RealTime : MonoBehaviour
{
    public GameObject hour_hand;
    public GameObject minute_hand;
    DateTime time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        minute_hand.transform.eulerAngles = new Vector3(0,90,0);
        time = System.DateTime.UtcNow.ToLocalTime();
        Debug.Log(time);
    }
}
