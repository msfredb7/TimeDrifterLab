﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Simulation
{
    public static void Initialize(ISimModuleBlueprintBank blueprintBank)
    {
        SimModules.blueprintBank = blueprintBank;
        SimModules.entityManager = new SimModuleEntityManager();
        SimModules.sceneLoader = new SimModuleSceneLoader();
        SimModules.serializer = new SimModuleSerializer();
        SimModules.ticker = new SimModuleTicker();
        SimModules.world = new SimWorld();
        SimModules.worldSearcher = new SimModuleWorldSearcher();
    }

    public static void Shutdown()
    {
        SimModules.blueprintBank = null;
        SimModules.entityManager = null;
        SimModules.sceneLoader = null;
        SimModules.serializer = null;
        SimModules.ticker = null;
        SimModules.world = null;
        SimModules.worldSearcher = null;
    }

    /// <summary>
    /// Is the simulation ready to run ?
    /// </summary>
    public static bool isInitialized => SimModules.world != null;

    /// <summary>
    /// Can the simulation be ticked ? Some things can prevent the simulation to be ticked (like being in the middle of a scene injection)
    /// </summary>
    public static bool canBeTicked => SimModules.ticker.canSimBeTicked;

    /// <summary>
    /// Can the simulation be saved ? Some things can prevent the simulation to be saved (like being in the middle of a scene injection)
    /// </summary>
    public static bool canBeSaved => SimModules.serializer.canSimWorldBeSaved;

    /// <summary>
    /// The simulation tick delta time. Should be constant for the simulation to stay deterministic
    /// </summary>
    public static readonly Fix64 deltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds

    /// <summary>
    /// The current simulation tick id. Increments by 1 every Tick()
    /// </summary>
    public static uint tickId => SimModules.world.tickId;
    public static void Tick(in SimTickData tickData) => SimModules.ticker.Tick(tickData);

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original) => SimModules.entityManager.Instantiate(original);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, Transform parent) => SimModules.entityManager.Instantiate(original, parent);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation) => SimModules.entityManager.Instantiate(original, position, rotation);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransform parent) => SimModules.entityManager.Instantiate(original, position, rotation, parent);

    /// <summary>
    /// Load the given scene and inject all gameobjects with the SimEntity component into the simulation
    /// </summary>
    public static void LoadScene(string sceneName) => SimModules.sceneLoader.LoadScene(sceneName);

    public static SimBlueprint GetBlueprint(in SimBlueprintId blueprintId) => SimModules.blueprintBank.GetBlueprint(blueprintId);

    public static SimEntity FindEntityWithName(string name) => SimModules.worldSearcher.FindEntityWithName(name);
    public static SimEntity FindEntityWithComponent<T>() where T : SimComponent => SimModules.worldSearcher.FindEntityWithComponent<T>();
    public static SimEntity FindEntityWithComponent<T>(out T comp) where T : SimComponent => SimModules.worldSearcher.FindEntityWithComponent(out comp);
    public static void ForEveryEntityWithComponent<T>(Action<T> action) where T : SimComponent => SimModules.worldSearcher.ForEveryEntityWithComponent(action);
}
