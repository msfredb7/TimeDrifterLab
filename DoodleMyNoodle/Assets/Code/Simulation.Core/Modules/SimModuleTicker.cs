﻿using System;
using System.Collections.Generic;

public class SimModuleTicker : IDisposable
{
    internal bool CanSimBeTicked =>
        SimModules.SceneLoader.PendingSceneLoads == 0
        && SimModules.EntityManager.PendingPermanentEntityDestructions == 0
        && !IsTicking;

    internal bool IsTicking = false;
    internal uint TickId => SimModules.World.TickId;

    internal List<ISimTickable> Tickables = new List<ISimTickable>();
    internal List<ISimTickable> NewTickables = new List<ISimTickable>();

    internal void Tick(in SimTickData tickData)
    {
        if (!CanSimBeTicked)
            throw new System.Exception("Tried to tick the simulation while it could not. Investigate.");

        IsTicking = true;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Call OnSimStart() methods on objects that need it                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        CallOnSimStartOnObjectNeedingIt();

        ////////////////////////////////////////////////////////////////////////////////////////
        //      INPUTS                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        foreach (SimInput input in tickData.inputs)
        {
            SimModules.InputProcessorManager.ProcessInput(input);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      TICK                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        Tickables.AddRange(NewTickables);
        NewTickables.Clear();
        for (int i = 0; i < Tickables.Count; i++)
        {
            if (Tickables[i].isActiveAndEnabled)
            {
                try
                {
                    Tickables[i].OnSimTick();
                }
                catch (Exception e)
                {
                    DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Call OnSimStart() methods on objects that need it                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        CallOnSimStartOnObjectNeedingIt();


        SimModules.World.TickId++;


        IsTicking = false;
    }

    internal void OnAddSimObjectToSim(SimObject obj)
    {
        if (obj is ISimTickable)
        {
            // Here we add the Tickable object in the NewTickables list instead of Tickables BECAUSE:
            // we want to make sure we call OnSimStart on all new objects before starting to tick them

            NewTickables.Add((ISimTickable)obj);
        }
    }

    internal void OnRemovingSimObjectFromSim(SimObject obj)
    {
        if (obj is ISimTickable)
        {
            // The Tickable object might be in either of those lists. Try removing it from both.

            if (NewTickables.Remove((ISimTickable)obj) == false)
            {
                Tickables.Remove((ISimTickable)obj);
            }
        }
    }

    void CallOnSimStartOnObjectNeedingIt()
    {
        List<SimObject> objs = SimModules.World.ObjectsThatHaventStartedYet;
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].isActiveAndEnabled)
            {
                try
                {
                    objs[i].OnSimStart();
                }
                catch (Exception e)
                {
                    DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
                }
                objs.RemoveWithLastSwapAt(i);
                i--;
            }
        }
    }

    public void Dispose()
    {
    }
}