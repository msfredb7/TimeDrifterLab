﻿using SimulationControl;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class MasterOnlyAttribute : Attribute
{

}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ClientOnlyAttribute : Attribute
{

}

[UpdateAfter(typeof(TickSimulationSystem))]
[UpdateInGroup(typeof(SimulationControlSystemGroup))]
public class ViewSystemGroup : ManualCreationComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }

    public void Initialize(SimulationControlSystemGroup simControlGroup)
    {
        // TODO: create/destroy systems + Initialize() + Shutdown() pattern
        ManualCreateAndAddSystem<BeginViewSystem>();
        ManualCreateAndAddSystem<EndViewSystem>(); 

        IEnumerable<Type> viewComponentSystemTypes =

                    // get all ViewComponent systems
                    TypeUtility.GetECSTypesDerivedFrom(typeof(ViewComponentSystem))
            .Concat(TypeUtility.GetECSTypesDerivedFrom(typeof(ViewJobComponentSystem)))
            .Concat(TypeUtility.GetECSTypesDerivedFrom(typeof(ViewEntityCommandBufferSystem)))

            // exlude those with the DisableAutoCreate attribute
            .Where((type) =>
            {
                if (type == null)
                    return false;

                if (Attribute.IsDefined(type, typeof(DisableAutoCreationAttribute), true))
                    return false;

                if (!simControlGroup.IsClient && Attribute.IsDefined(type, typeof(ClientOnlyAttribute), true))
                    return false;

                if (!simControlGroup.IsMaster && Attribute.IsDefined(type, typeof(MasterOnlyAttribute), true))
                    return false;

                return true;
            });


        foreach (var systemType in viewComponentSystemTypes)
        {
            ManualCreateAndAddSystem(systemType);
        }
    }

    public void Shutdown()
    {
        DestroyAllManuallyCreatedSystems();
    }

    public override void SortSystemUpdateList()
    {
        base.SortSystemUpdateList();

        int i = m_systemsToUpdate.IndexOf(World.GetExistingSystem<BeginViewSystem>());
        if (i != -1)
            m_systemsToUpdate.MoveFirst(i);

         i = m_systemsToUpdate.IndexOf(World.GetExistingSystem<EndViewSystem>());
        if (i != -1)
            m_systemsToUpdate.MoveLast(i);

    }

    protected override void OnUpdate()
    {
        if (CanUpdate)
            base.OnUpdate();
    }
}


// This could be moved to CCC > ECS > Systems
public abstract class ManualCreationComponentSystemGroup : ComponentSystemGroup
{
    private List<ComponentSystemBase> _manuallyCreatedSystems = new List<ComponentSystemBase>();

    protected ComponentSystemBase ManualCreateAndAddSystem(Type type)
    {
        if (!Attribute.IsDefined(type, typeof(DisableAutoCreationAttribute)) &&
            !Attribute.IsDefined(type.Assembly, typeof(DisableAutoCreationAttribute)))
        {
            DebugService.LogError($"We should not be manually creating the system {type} since its going to create itself anyway");
        }

        var sys = World.GetOrCreateSystem(type);
        _manuallyCreatedSystems.Add(sys);
        AddSystemToUpdateList(sys);
        return sys;
    }
    protected T ManualCreateAndAddSystem<T>() where T : ComponentSystem
    {
        return ManualCreateAndAddSystem(typeof(T)) as T;
    }

    protected void DestroyAllManuallyCreatedSystems()
    {
        foreach (var sys in _manuallyCreatedSystems)
        {
            RemoveSystemFromUpdateList(sys);
            World.DestroySystem(sys);
        }
        _manuallyCreatedSystems.Clear();
    }
}