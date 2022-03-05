using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    public Transform[] way_points;
    public float3[] wps;
    public float3[] gun_locations;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        wps = new float3[way_points.Length];

        for(int i = 0; i < way_points.Length; i++)
        {
            wps[i] = way_points[i].position;
        }
    }
}