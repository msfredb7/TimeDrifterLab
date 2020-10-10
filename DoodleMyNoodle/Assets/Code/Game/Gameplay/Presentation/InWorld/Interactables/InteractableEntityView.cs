using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class InteractableEntityView : BindedPresentationEntityComponent
{
    protected bool _previousInteractedState = false;

    protected override void OnGamePresentationUpdate() { }

    public override void OnPostSimulationTick()
    {
        Entity InteractableEntity = SimEntity;
        if (InteractableEntity != Entity.Null)
        {
            if (SimWorld.TryGetComponentData(InteractableEntity, out Interacted interactedData))
            {
                if (interactedData.Value != _previousInteractedState)
                {
                    if (interactedData.Value)
                    {
                        OnInteractionTriggeredByInput();
                    }
                    else
                    {
                        OnInteractionReset();
                    }

                    _previousInteractedState = interactedData.Value;
                }
            }
        }
    }

    protected virtual void OnInteractionTriggeredByInput() { }

    protected virtual void OnInteractionReset() { }

    protected bool CanTrigger()
    {
        Entity interactable = SimEntity;
        if (interactable == Entity.Null)
        {
            return false;
        }

        if (SimWorld.TryGetComponentData(interactable, out Interactable interactableData))
        {
            if (SimWorld.TryGetComponentData(interactable, out Interacted interactedData))
            {
                return !interactedData.Value;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}