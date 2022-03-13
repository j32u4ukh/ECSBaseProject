using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public static class GameDataManager
{
    public static Vector3 player_psotion;

    public static float strength1;
    public static float scale1;
    public static float strength2;
    public static float scale2;
    public static float strength3;
    public static float scale3;

    public static Entity sand_entity;
    public static Entity dirt_entity;
    public static Entity grass_entity;
    public static Entity rock_entity;
    public static Entity snow_entity;

    public static float sand_altitude;
    public static float dirt_altitude;
    public static float grass_altitude;
    public static float rock_altitude;
    public static float snow_altitude;

    public static bool changed_flag = false;

    public static float3[] bullet_points;
}
