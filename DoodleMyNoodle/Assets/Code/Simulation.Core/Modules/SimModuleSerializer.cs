﻿using Newtonsoft.Json;
using Sim.Operations;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;

internal class SimModuleSerializer : SimModuleBase
{
    //internal bool CanSimWorldBeSaved =>
    //    SimModules._SceneLoader.PendingSceneLoads == 0
    //    && SimModules._Ticker.IsTicking == false
    //    && IsInDeserializationProcess == false;

    //internal bool CanSimWorldBeLoaded =>
    //    SimModules._SceneLoader.PendingSceneLoads == 0
    //    && SimModules._Ticker.IsTicking == false
    //    && IsInDeserializationProcess == false;

    //internal bool IsInDeserializationProcess =>
    //    _deserializationOperation != null
    //    && _deserializationOperation.IsRunning;

    //internal bool IsInSerializationProcess =>
    //    _serializationOperation != null
    //    && _serializationOperation.IsRunning;

    //internal Action<SimSerializationResult> OnSerializationResult;
    //internal Action<SimDeserializationResult> OnDeserializationResult;

    //SimDeserializationOperation _deserializationOperation;
    //SimSerializationOperationWithCache _serializationOperation;

    static JsonSerializerSettings s_cachedJsonSettings;
    static SimObjectJsonConverter s_cachedSimObjectJsonConverter;

    static JsonSerializerSettings GetJsonSettings()
    {
        if (s_cachedJsonSettings == null)
        {
            s_cachedSimObjectJsonConverter = new SimObjectJsonConverter();

            s_cachedJsonSettings = new JsonSerializerSettings();

            s_cachedJsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            s_cachedJsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            s_cachedJsonSettings.TypeNameHandling = TypeNameHandling.Auto;
            s_cachedJsonSettings.Converters = new List<JsonConverter>()
            {
                s_cachedSimObjectJsonConverter,
                new IDTypeJsonConverter(),
                new Fix64JsonConverter(),
            };

            var contractResolver = new CustomJsonContractResolver();
#pragma warning disable CS0618 // Type or member is obsolete
            contractResolver.DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
#pragma warning restore CS0618 // Type or member is obsolete

            s_cachedJsonSettings.ContractResolver = contractResolver;
            s_cachedJsonSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
        }
        return s_cachedJsonSettings;
    }

    static SimObjectJsonConverter GetSimObjectJsonConverter()
    {
        GetJsonSettings();
        return s_cachedSimObjectJsonConverter;
    }

    public static SimSerializationOperationWithCache SerializeSimulation(World simulationWorld)
    {
        //if (!SimModules._Serializer.CanSimWorldBeSaved)
        //{
        //    DebugService.LogError("Cannot serialize SimWorld right now. We must not be ticking nor loading a scene");
        //    return null;
        //}

        var _serializationOperation = new SimSerializationOperationWithCache(GetSimObjectJsonConverter(), GetJsonSettings(), simulationWorld);

        //_serializationOperation.OnFailCallback = (op) =>
        //{
        //    OnSerializationResult?.Invoke(new SimSerializationResult()
        //    {
        //        SuccessLevel = SimSerializationResult.SuccessType.Failed,
        //    });
        //};

        //_serializationOperation.OnSucceedCallback = (op) =>
        //{
        //    //SimSerializationOperationWithCache serializationOp = (SimSerializationOperationWithCache)op;
        //    //OnSerializationResult?.Invoke(new SimSerializationResult()
        //    //{
        //    //    SuccessLevel = serializationOp.PartialSuccess ?
        //    //        SimSerializationResult.SuccessType.PartialSuccess :
        //    //        SimSerializationResult.SuccessType.Succeeded,
        //    //    Data = serializationOp.SerializationData
        //    //});
        //};

        _serializationOperation.Execute();

        return _serializationOperation;
    }

    public static SimDeserializationOperation DeserializeSimulation(string data, World simulationWorld)
    {
        //if (!CanSimWorldBeLoaded)
        //{
        //    DebugService.LogError("Cannot deserialize SimWorld right now. We must not be ticking nor loading a scene");
        //    return null;
        //}

        var _deserializationOperation = new SimDeserializationOperation(data, GetSimObjectJsonConverter(), GetJsonSettings(), simulationWorld);

        //_deserializationOperation.OnFailCallback = (op) =>
        //{
        //    OnDeserializationResult?.Invoke(new SimDeserializationResult()
        //    {
        //        SuccessLevel = SimDeserializationResult.SuccessType.Failed,
        //    });
        //};

        //_deserializationOperation.OnSucceedCallback = (op) =>
        //{
        //    OnDeserializationResult?.Invoke(new SimDeserializationResult()
        //    {
        //        SuccessLevel = ((SimDeserializationOperation)op).PartialSuccess ?
        //            SimDeserializationResult.SuccessType.PartialSuccess :
        //            SimDeserializationResult.SuccessType.Succeeded
        //    });
        //};

        _deserializationOperation.Execute();

        return _deserializationOperation;
    }

    public override void Dispose()
    {
        base.Dispose();

        //if (_deserializationOperation != null && _deserializationOperation.IsRunning)
        //    _deserializationOperation.TerminateWithFailure();

        //if (_serializationOperation != null && _serializationOperation.IsRunning)
        //    _serializationOperation.TerminateWithFailure();
    }
}