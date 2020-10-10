using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ItemContainerInteractableByClick : ObjectInteractableByClick
{
    private bool _inventoryDisplayed = false;

    protected override void OnInteractionTriggeredByInput()
    {
        if (SimWorldCache.LocalPawn == Entity.Null || SimEntity == Entity.Null)
            return;

        if (SimWorld.TryGetComponentData(SimEntity, out Interacted interacted))
        {
            if (SimWorldCache.LocalPawn == interacted.Instigator)
            {
                _inventoryDisplayed = true;
                InteractableInventoryDisplaySystem.Instance.SetupDisplayForInventory(SimEntity);
            }
        }
    }

    protected override void OnGamePresentationUpdate() 
    {
        if (SimWorldCache.LocalPawn == Entity.Null || SimEntity == Entity.Null || !InteractableInventoryDisplaySystem.Instance.IsOpen())
        {
            _inventoryDisplayed = false;
            return;
        }

        if (SimWorld.TryGetComponentData(SimEntity, out Interacted interacted))
        {
            if (_inventoryDisplayed && SimWorldCache.LocalPawn == interacted.Instigator)
            {
                fix3 itemContainerPosition = SimWorld.GetComponentData<FixTranslation>(SimEntity);
                fix3 localPawnPosition = SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn);

                fix interactionTileRange = SimWorld.GetComponentData<Interactable>(SimEntity).Range;

                int tilesBetween = fix.RoundToInt(fix.Abs((itemContainerPosition.x - localPawnPosition.x) + (itemContainerPosition.y - localPawnPosition.y)));

                if ((tilesBetween > interactionTileRange) && InteractableInventoryDisplaySystem.Instance.IsOpen())
                {
                    _inventoryDisplayed = false;
                    InteractableInventoryDisplaySystem.Instance.CloseDisplay();
                }
            }
        }
    }
}