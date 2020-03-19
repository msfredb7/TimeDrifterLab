﻿using Unity.Entities;

public static class EntityManagerExtensions
{
    public static bool TryGetComponentData<T>(this EntityManager entityManager, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (entityManager.HasComponent<T>(entity))
        {
            componentData = entityManager.GetComponentData<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }
}