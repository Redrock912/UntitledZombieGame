using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{


    public RawImage[] rawImages;

    public Text[] itemCounts;

    KeyCode[] keyCodes;
    // Start is called before the first frame update

    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        keyCodes = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        UseItem(0);
        TurnSlotOff(0);
    }

    // Update is called once per frame
    void Update()
    {

        for(int i=0; i< keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                UseItem(i);
                TurnSlotOff(i);
            }
        }

        DisplayCurrentItems();
        
    }

    void UseItem(int inventorySlotNumber)
    {
        player.currentSlotNumber = inventorySlotNumber;
        rawImages[inventorySlotNumber].color = Color.red;

    }

    void TurnSlotOff(int inventorySlotNumber)
    {
        for(int i=1;i <= 3; i++)
        {
            // 0 -> 1 2 3 , 1 -> 2 3 0 , 2 -> 3 0 1 , 3 -> 0 1 2
            rawImages[(inventorySlotNumber + i) % 4].color = Color.white;
        }
    }

    void DisplayCurrentItems()
    {
        for(int i=0;i<player.currentItems.Length; i++)
        {
            itemCounts[i].text = player.currentItems[i].ToString();
        }
    }
}
