﻿using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionIdAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Value;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionId()
        {
            Value = GameActionBank.GetActionId(GameActionBank.GetAction(Value))
        });
    }
}