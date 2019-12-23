using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingZombieUI : MonoBehaviour
{
    

    public GameObject bigZombieImage;
    public GameObject[] zombieImage;
    public Text zombieCount;

    int currentZombie = 0;
    Zombie[] zombieObjects;
    private void Awake()
    {
        zombieObjects = FindObjectsOfType<Zombie>();
        currentZombie = zombieObjects.Length;
    }


    private void Start()
    {
        foreach (var i in zombieObjects)
        {
            i.OnZombieDeath += OnZombieDeath;
        }
    }
    void OnZombieDeath()
    {
        currentZombie--;
        if (currentZombie == 5)
        {
            ChangeToSmallImages();
        }

        print(currentZombie);
    }


    void ChangeToSmallImages()
    {
        bigZombieImage.SetActive(false);

        foreach (var z in zombieImage)
        {
            z.SetActive(true);
        }
    }

    void SetOneByOne(int zombieCount)
    {

        zombieImage[zombieCount].SetActive(false);
    }

    private void Update()
    {
        if (currentZombie <= 4)
        {
            SetOneByOne(currentZombie);
        }
        else
        {
            zombieCount.text = currentZombie.ToString();
        }
    }
}
