﻿using SimulationControl;
using Unity.Entities;

/// <summary>
/// Those helper methods are meant to be used EXCLUSIVELY from outside of the ECS world
/// </summary>
public static class GameMonoBehaviourHelpers
{
    public static World PresentationWorld => World.DefaultGameObjectInjectionWorld;
    public static World SimulationWorld => GetPresentationWorldSystem<SimulationWorldSystem>()?.SimulationWorld;

    public static void SubmitInput(SimInput input, bool throwErrorIfFailed = false)
    {
        var submitSystem = PresentationWorld.GetExistingSystem<SubmitSimulationInputSystem>();
        if (submitSystem != null)
        {
            submitSystem.SubmitInput(input);
        }
        else
        {
            if (throwErrorIfFailed)
                throw new System.Exception($"Failed to submit input: {input}");
        }
    }

    public static T GetPresentationWorldSystem<T>() where T : ComponentSystem
    {
        return PresentationWorld.GetExistingSystem<T>();
    }
}