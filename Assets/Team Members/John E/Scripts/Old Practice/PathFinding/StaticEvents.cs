using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticEvents : MonoBehaviour
{
    public delegate void ReScan(GameObject go);
    public static event ReScan ReScanEvent;

    public static void ReScanEventTest(GameObject go)
    {
        ReScanEvent?.Invoke(go);
    }
}
