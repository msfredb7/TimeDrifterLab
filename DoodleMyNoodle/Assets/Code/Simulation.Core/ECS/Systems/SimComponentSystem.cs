﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public abstract class SimComponentSystem : ComponentSystem
{
    /// <summary>
    /// The current Time data for this system's world.
    /// </summary>
    public new ref readonly FixTimeData Time => ref World.FixTime;

    public new SimulationWorld World { get; private set; }
    public ISimWorldReadWriteAccessor Accessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        World = ((SimulationWorld)base.World);
        Accessor = World.InternalAccessor;
    }
}

public abstract class SimJobComponentSystem : JobComponentSystem
{
    /// <summary>
    /// The current Time data for this system's world.
    /// </summary>
    public new ref readonly FixTimeData Time => ref World.FixTime;

    public new SimulationWorld World { get; private set; }
    public ISimWorldReadWriteAccessor Accessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        World = ((SimulationWorld)base.World);
        Accessor = World.InternalAccessor;
    }
}


// should we use this instead ?

//[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
//public class CreateInSimWorldAttribute : Attribute
//{

//}