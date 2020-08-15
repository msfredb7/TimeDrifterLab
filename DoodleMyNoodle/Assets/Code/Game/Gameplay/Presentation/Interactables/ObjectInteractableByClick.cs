using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : BindedPresentationEntityComponent
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _previousInteractedState = false;
    private bool _highlighted = false;

    protected override void OnGamePresentationUpdate() { }

    public override void OnPostSimulationTick()
    {
        if (SimWorld.TryGetComponentData(SimEntity, out Interacted interacted))
        {
            if (interacted != _previousInteractedState)
            {
                if (interacted)
                {
                    OnInteractionTriggeredByInput();
                }

                _previousInteractedState = interacted.Value;
            }

            if (interacted)
            {
                SetHighlighted(false);
            }
        }
    }

    protected virtual void OnInteractionTriggeredByInput() { }

    private void OnMouseOver()
    {
        if (CanTrigger())
        {
            SetHighlighted(true);
        }
    }

    private void OnMouseExit()
    {
        SetHighlighted(false);
    }

    private void OnMouseDown()
    {
        if (CanTrigger())
        {
            // TODO : One day once it's fix send directly the entity
            // BindedSimEntityManaged bindingEntityComponent = GetComponent<BindedSimEntityManaged>();
            // bindingEntityComponent.SimEntity

            SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
            SimWorld.SubmitInput(inputUseInteractable);
        }
    }

    private void SetHighlighted(bool highlighted)
    {
        if (_highlighted == highlighted)
            return;

        _highlighted = highlighted;

        if (highlighted)
        {
            HighlightService.Params args = HighlightService.Params.Default;

            args.Duration = HighlightService.Duration.Long;
            args.FlickerSpeed = HighlightService.FlickerSpeed.Slow;
            args.Intensity = HighlightService.Intensity.Normal;
            args.AnimStart = HighlightService.AnimStart.MidHighlight;

            HighlightService.HighlightSprite(_spriteRenderer, args);
        }
        else
        {
            HighlightService.StopHighlight(_spriteRenderer);
        }
    }

    private bool CanTrigger()
    {
        Entity interactable = SimEntity;
        if (!SimWorld.Exists(interactable))
        {
            return false;
        }

        if (SimWorld.TryGetComponentData(interactable, out Interactable interactableData))
        {
            if (SimWorld.TryGetComponentData(interactable, out Interacted interactedData))
            {
                return interactableData.OnlyOnce ? !interactedData.Value : true;
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