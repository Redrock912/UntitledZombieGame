using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    #region Singleton

    public static ActorManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;

    public GameObject zombiePartPrefab;
}
