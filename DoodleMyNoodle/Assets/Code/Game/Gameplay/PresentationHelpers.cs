﻿using CCC.Fix2D;
using SimulationControl;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class PresentationHelpers
{
    public static World PresentationWorld => World.DefaultGameObjectInjectionWorld;
    public static ExternalSimWorldAccessor GetSimulationWorld() => GetPresentationWorldSystem<SimulationWorldSystem>()?.SimWorldAccessor;

    public static void SubmitInput(SimInput input, bool throwErrorIfFailed = false)
    {
        var submitSystem = PresentationWorld?.GetExistingSystem<SubmitSimulationInputSystem>();
        if (submitSystem != null)
        {
            submitSystem.SubmitInput(input);
        }
        else
        {
            if (throwErrorIfFailed)
                throw new System.Exception($"Failed to submit input: {input}. Could not find '{nameof(SubmitSimulationInputSystem)}'");
        }
    }

    public static T GetPresentationWorldSystem<T>() where T : ComponentSystem
    {
        return PresentationWorld?.GetExistingSystem<T>();
    }

    public static GameObject FindSimAssetPrefab(SimAssetId simAssetId)
    {
        return SimAssetBankInstance.GetLookup().GetSimAsset(simAssetId)?.gameObject;
    }

    public static ItemAuth FindItemAuth(SimAssetId itemID)
    {
        GameObject itemPrefab = FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            return itemPrefab.GetComponent<ItemAuth>();
        }
        return null;
    }

    public static GameObject FindBindedView(Entity simEntity)
    {
        if (BindedSimEntityManaged.InstancesMap.TryGetValue(simEntity, out GameObject result))
        {
            return result;
        }
        return null;
    }

    public static Quaternion SimRotationToUnityRotation(FixRotation fixRotation)
    {
        return SimRotationToUnityRotation(fixRotation.Value);
    }

    public static Quaternion SimRotationToUnityRotation(fix radAngle)
    {
        return Quaternion.Euler(0, 0, math.degrees((float)radAngle));
    }

    public static void RequestFloatingText(Vector2 position, string text, Color color)
    {
        FloatingTextSystem.Instance.RequestText(position, text, color);
    }

    public static void ResizeGameObjectList(List<GameObject> gameObjectList, int count, GameObject prefab, Transform container, Action<GameObject> onCreate = null, Action<GameObject> onDestroy = null)
    {
        while (gameObjectList.Count < count)
        {
            var newObject = UnityEngine.Object.Instantiate(prefab, container);
            gameObjectList.Add(newObject);
            onCreate?.Invoke(newObject);
        }

        while (gameObjectList.Count > count)
        {
            int i = gameObjectList.Count - 1;
            onDestroy?.Invoke(gameObjectList[i]);
            UnityEngine.Object.Destroy(gameObjectList[i]);
            gameObjectList.RemoveAt(i);
        }
    }

    public static void ResizeGameObjectList<T>(List<T> componentList, int count, T prefab, Transform container, Action<T> onCreate = null, Action<T> onDestroy = null) where T : UnityEngine.Component
    {
        while (componentList.Count < count)
        {
            var newObject = UnityEngine.Object.Instantiate(prefab, container);
            componentList.Add(newObject);
            onCreate?.Invoke(newObject);
        }

        while (componentList.Count > count)
        {
            int i = componentList.Count - 1;
            onDestroy?.Invoke(componentList[i]);
            UnityEngine.Object.Destroy(componentList[i].gameObject);
            componentList.RemoveAt(i);
        }
    }

    public static void UpdateGameObjectList<T, U>(List<T> componentList, List<U> data, T prefab, Transform container, Action<T, U> onCreate = null, Action<T, U> onUpdate = null, Action<T> onDeactivate = null) where T : UnityEngine.Component
    {
        int i = 0;
        for (; i < data.Count; i++)
        {
            if (i >= componentList.Count)
            {
                var newObject = UnityEngine.Object.Instantiate(prefab, container);
                componentList.Add(newObject);
                onCreate?.Invoke(newObject, data[i]);
            }

            componentList[i].gameObject.SetActive(true);
            onUpdate?.Invoke(componentList[i], data[i]);
        }

        for (int r = componentList.Count - 1; r >= i; r--)
        {
            onDeactivate?.Invoke(componentList[r]);
            componentList[r].gameObject.SetActive(false);
        }
    }

    public static class Surveys
    {
        public static GameAction GetGameAction(ISimWorldReadAccessor simWorld, Entity item)
        {
            if (simWorld == null)
                return null;

            if (!simWorld.Exists(item))
                return null;

            if (!simWorld.TryGetComponent(item, out GameActionId gameActionId))
                return null;

            return GameActionBank.GetAction(gameActionId);
        }

        public static T GetGameAction<T>(ISimWorldReadAccessor simWorld, Entity item) where T : GameAction
        {
            return GetGameAction(simWorld, item) as T;
        }

        public static bool GetItemTrajectorySettings(GamePresentationCache cache, GameAction.UseContext useContext, Vector2 direction,
            out Vector2 spawnOffset,
            out float radius)
        {
            spawnOffset = Vector2.zero;
            radius = 0.05f;

            GameActionThrow throwAction = GetGameAction<GameActionThrow>(cache.SimWorld, useContext.Item);

            if (throwAction != null)
            {
                spawnOffset = (Vector2)throwAction.GetSpawnPosOffset(cache.SimWorld, useContext, (fix2)direction);
                radius = (float)throwAction.GetProjectileRadius(cache.SimWorld, useContext);
                return true;
            }

            GameActionDirectionalJump jumpAction = GetGameAction<GameActionDirectionalJump>(cache.SimWorld, useContext.Item);

            if (jumpAction != null)
            {
                spawnOffset = Vector2.zero;
                radius = (float)CommonReads.GetActorRadius(cache.SimWorld, useContext.InstigatorPawn);
                return true;
            }

            return false;
        }

        public static float GetProjectileGravityScale(GamePresentationCache cache, GameAction.UseContext useContext)
        {
            if (cache.SimWorld == null)
                return 1;

            if (!cache.SimWorld.Exists(useContext.Item))
                return 1;

            if (cache.SimWorld.TryGetComponent(useContext.Item, out GameActionThrow.Settings throwSettings))
            {
                return GetEntityGravScale(throwSettings.ProjectilePrefab);
            }

            return 1;

            float GetEntityGravScale(Entity entity)
            {
                if (!cache.SimWorld.Exists(entity))
                    return 1;

                if (!cache.SimWorld.TryGetComponent(entity, out PhysicsGravity grav))
                    return 1;

                return (float)grav.Scale;
            }
        }
    }
}