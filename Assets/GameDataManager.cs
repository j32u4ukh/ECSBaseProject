using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    public Transform player;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}
