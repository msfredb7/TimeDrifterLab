﻿using Unity.Entities;
using static Unity.Mathematics.math;

public interface IStatInt
{
    int Value { get; set; }
}

public interface IStatFix
{
    Fix64 Value { get; set; }
}

public struct MinimumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct MaximumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct MinimumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public Fix64 Value;
}

public struct MaximumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public Fix64 Value;
}

internal static partial class CommonWrites
{
    public static void SetStatInt<T>(World world, Entity entity, T compData)
        where T : struct, IComponentData, IStatInt
    {
        if (world.EntityManager.TryGetComponentData(entity, out MinimumInt<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (world.EntityManager.TryGetComponentData(entity, out MaximumInt<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        world.EntityManager.SetComponentData(entity, compData);
    }

    public static void SetStatFix<T>(World world, Entity entity, T compData)
    where T : struct, IComponentData, IStatFix
    {
        if (world.EntityManager.TryGetComponentData(entity, out MinimumFix<T> minimum))
        {
            compData.Value = FixMath.Max(minimum.Value, compData.Value);
        }

        if (world.EntityManager.TryGetComponentData(entity, out MaximumFix<T> maximum))
        {
            compData.Value = FixMath.Min(maximum.Value, compData.Value);
        }

        world.EntityManager.SetComponentData(entity, compData);
    }
}
