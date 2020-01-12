﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject InventoryTileContainer;
    public GameObject InventoryTilePrefab;

    private bool _hasBeenSetup = false;

    public void ToggleInventory()
    {
        if (!_hasBeenSetup)
        {
            _hasBeenSetup = true;

            SimPawnComponent PlayerPawn = SimPawnHelpers.GetPawnFromController(PlayerIdHelpers.GetLocalSimPlayerComponent());

            SimInventoryComponent Inventory = PlayerPawn.GetComponent<SimInventoryComponent>();

            for (int i = 0; i < Inventory.InventorySize; i++)
            {
                SimItem item = Inventory.GetItem(i);
                InventorySlot newSlot = Instantiate(InventoryTilePrefab, InventoryTileContainer.transform).GetComponent<InventorySlot>();
                newSlot.Init(item);
            }
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
