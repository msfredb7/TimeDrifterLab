﻿using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

[MasterOnly]
public class AssignSimPlayersSystem : ViewComponentSystem
{
    Dictionary<PlayerInfo, double> _dontAskForPlayerCreateCooldownMap = new Dictionary<PlayerInfo, double>();

    protected override void OnUpdate()
    {
        if (PlayerRepertoireSystem.Instance == null)
            return;

        // When we submit an input in the simulation, we have no guarantee that it will be processed in the next frame
        // This 'cooldown' mechanism ensures we don't ask for too many player creations
        if (_dontAskForPlayerCreateCooldownMap.Count > 0)
        {
            foreach (KeyValuePair<PlayerInfo, double> playerInCooldown in _dontAskForPlayerCreateCooldownMap)
            {
                if(playerInCooldown.Value < Time.ElapsedTime)
                {
                    _dontAskForPlayerCreateCooldownMap.Remove(playerInCooldown.Key);
                    break;
                }
            }
        }

        foreach (PlayerInfo playerInfo in PlayerRepertoireSystem.Instance.Players)
        {
            if(playerInfo.SimPlayerId == PersistentId.Invalid)
            {
                Entity unassignedSimPlayer = GetUnassignedSimPlayer();

                if (unassignedSimPlayer == Entity.Null)
                {
                    // ask the simulation to create a new player

                    // When we submit an input in the simulation, we have no guarantee that it will be processed in the next frame
                    // This 'cooldown' mechanism ensures we don't ask for too many player creations
                    if (!_dontAskForPlayerCreateCooldownMap.ContainsKey(playerInfo))
                    {
                        SimWorldAccessor.SubmitInput(new SimInputPlayerCreate() { PlayerName = playerInfo.PlayerName });
                        _dontAskForPlayerCreateCooldownMap.Add(playerInfo, Time.ElapsedTime + 1);
                    }

                }
                else
                {
                    // assign the sim player to the player
                    PersistentId simPlayerId = SimWorldAccessor.GetComponentData<PersistentId>(unassignedSimPlayer);

                    PlayerRepertoireMaster.Instance.AssignSimPlayerToPlayer(playerInfo.PlayerId, simPlayerId);
                }
            }
        }
    }

    Entity GetUnassignedSimPlayer()
    {
        using (EntityQuery query = SimWorldAccessor.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<PersistentId>()))
        {
            using (NativeArray<Entity> simPlayers = query.ToEntityArray(Allocator.TempJob))
            {
                foreach (Entity playerEntity in simPlayers)
                {
                    if (PlayerIdHelpers.GetPlayerFromSimPlayer(playerEntity, SimWorldAccessor) == null)
                    {
                        return playerEntity;
                    }
                }
            }
        }


        return Entity.Null;
    }
}
