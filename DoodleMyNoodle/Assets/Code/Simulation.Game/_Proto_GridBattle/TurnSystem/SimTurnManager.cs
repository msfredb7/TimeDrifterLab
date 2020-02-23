﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    AI
}

public class SimTurnManager : SimSingleton<SimTurnManager>, ISimTickable
{
    public const int TEAM_COUNT = 2;


    [System.Serializable]
    struct SerializedData
    {
        public int DurationOfATurn;
        public Fix64 Timer;
        public Team CurrentTeam;
    }

    public int DurationOfATurn => _data.DurationOfATurn;
    public Team CurrentTeam => _data.CurrentTeam;
    public Fix64 TurnRemainingTime => _data.Timer;

    public override void OnSimStart() 
    {
        base.OnSimStart();

        _data.Timer = DurationOfATurn;

        SwitchTurn();
    }

    void ISimTickable.OnSimTick()
    {
        _data.Timer -= Simulation.DeltaTime;

        if (_data.Timer <= 0)
        {
            SwitchTurn();
            if (_data.CurrentTeam == Team.AI)
                _data.Timer = 1;
            else
                _data.Timer = DurationOfATurn;
        }
    }

    private void SwitchTurn()
    {
        _data.CurrentTeam = _data.CurrentTeam + 1;
        if((int)_data.CurrentTeam >= TEAM_COUNT)
        {
            _data.CurrentTeam = 0;
        }
    }

    public bool IsMyTurn(Team myTeam)
    {
        return _data.CurrentTeam == myTeam;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        DurationOfATurn = 3,
        CurrentTeam = (Team)TEAM_COUNT
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
