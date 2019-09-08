﻿using System;
using System.Collections.Generic;

public class SimWorld : IDisposable
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      Everything in here should be synchronized across players. 
    //      If we ever save a game, that's all the data we should save.                                 
    ////////////////////////////////////////////////////////////////////////////////////////

    // the next tick the simulation is going to perform.
    internal uint TickId = 0;

    // the list of all entities in the simulation
    internal List<SimEntity> Entities = new List<SimEntity>();

    // should start at 0 for all games
    internal SimEntityId NextEntityId = SimEntityId.FirstValid;

    // fbessette:   since the "join in progress and download the sim from the server" feature is not implemented yet, 
    //              the seed will always be at 0 for all players. This means pressing "play" will always give the same random results at first.
    internal int Seed = 0;

    internal List<SimObject> ObjectsThatHaventStartedYet = new List<SimObject>(); // objects that never received their OnSimStart method called

    public void Dispose()
    {
        // nothing to do, not logic should be here
    }
}