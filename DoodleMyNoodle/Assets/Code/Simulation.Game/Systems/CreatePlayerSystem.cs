﻿using Unity.Entities;
using Unity.Collections;
using UnityEngineX;

[NetSerializable]
public class SimInputPlayerCreate : SimMasterInput
{
    public string PlayerName;
}

public class CreatePlayerSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimInputPlayerCreate createPlayerInput)
            {
                var newPlayerEntity = EntityManager.CreateEntity(
                    typeof(PlayerTag),
                    typeof(PersistentId),
                    typeof(Name),
                    typeof(ControlledEntity),
                    typeof(Team),
                    typeof(ReadyForNextTurn),
                    typeof(Active));

                // set persistent id
                EntityManager.SetComponentData(newPlayerEntity, this.MakeUniquePersistentId());

                // cap player name at 30 characters
                string playerName = createPlayerInput.PlayerName;
                if (playerName.Length > 30)
                    playerName = playerName.Substring(0, 30);

                // set name
                EntityManager.SetComponentData(newPlayerEntity, new Name() { Value = playerName });

                // assign controllable entity if possible
                Entity uncontrolledEntity = FindUncontrolledPawn();
                if (uncontrolledEntity != Entity.Null)
                {
                    EntityManager.SetComponentData(newPlayerEntity, new ControlledEntity() { Value = uncontrolledEntity });
                    EntityManager.SetComponentData(uncontrolledEntity, new Controllable() { CurrentController = newPlayerEntity });
                }

                // set team
                EntityManager.SetComponentData(newPlayerEntity, new Team() { Value = 0 });

                // set 'not ready for next turn'
                EntityManager.SetComponentData(newPlayerEntity, new ReadyForNextTurn() { Value = false });

                // set new player controller as currently active
                EntityManager.SetComponentData(newPlayerEntity, new Active() { Value = true });

                // FOR DEBUGGING ONLY
#if UNITY_EDITOR
                EntityManager.SetName(newPlayerEntity, $"Player({playerName})");
#endif
            }

            if (input is SimInputSetPlayerActive setPlayerActiveInput)
            {
                Entity PlayerEntity = CommonReads.FindPlayerEntity(Accessor, setPlayerActiveInput.PlayerID);

                if (PlayerEntity != Entity.Null)
                {
                    EntityManager.SetComponentData(PlayerEntity, new Active() { Value = setPlayerActiveInput.IsActive });
                }
            }
        }
    }

    private Entity FindUncontrolledPawn()
    {
        Entity uncontrolledEntity = Entity.Null;

        Entities
            .WithNone<InstantiateAndUseDefaultControllerTag>() // entities with this tag will have their DefaultController spawned
            .WithAll<Controllable>()
            .ForEach((Entity controllableEntity) =>
        {
            if (!CommonReads.IsPawnControlled(Accessor, controllableEntity))
            {
                uncontrolledEntity = controllableEntity;
                return;
            }
        });

        return uncontrolledEntity;
    }
}

public partial class CommonReads
{
    public static Entity GetPawnController(ISimWorldReadAccessor accessor, Entity pawn)
    {
        return accessor.GetComponentData<Controllable>(pawn).CurrentController;
    }

    public static bool IsPawnControlled(ISimWorldReadAccessor accessor, Entity pawn)
    {
        return GetPawnController(accessor, pawn) != Entity.Null;
    }
}
