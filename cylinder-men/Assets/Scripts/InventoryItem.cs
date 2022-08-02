using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }
    void OnPickup();
}

// public class InventoryEventArgs : EventArgs
// {
//     pulic Inventory
// }