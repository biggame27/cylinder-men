using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IInventoryItem
{
    public string Name
    {
        get
        {
            return "Pistol";
        }
    }

    public Sprite _Image = null;

    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }

    public void OnPickup()
    {
        //add more logic 
        gameObject.SetActive(false);
    }

}
