﻿using Unity.Entities;

[NetSerializable(baseClass = true)]
public abstract class SimPlayerInput : SimInput
{
    // this will be assigned by the server when its about to enter the simulation
    public PersistentId SimPlayerId;

    public override string ToString()
    {
        return $"{GetType().Name}(player:{SimPlayerId.Value})";
    }
}

[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class ExecutePlayerInputSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimPlayerInput playerInput)
            {
                Entity playerEntity = CommonReads.FindPlayerEntity(Accessor, playerInput.SimPlayerId);
                ExecutePlayerInput(playerInput, playerEntity);
            }
        }
    }

    private void ExecutePlayerInput(SimPlayerInput input, Entity playerEntity)
    {
        // fbessette: For now, we simply do a switch. 
        //            In the future, we'll probably want to implement something dynamic instead
        ExecutePawnControllerInputSystem pawnControllerInputSystem = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
        switch (input)
        {
            case SimPlayerInputSetStartingInventory setStartingInventoryInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputSetStartingInventory(playerEntity, setStartingInventoryInput.KitNumber));
                break;

            case SimPlayerInputSetPawnName setNameInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputSetPawnName(playerEntity, setNameInput.Name));
                break;

            case SimPlayerInputNextTurn nextTurnInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputNextTurn(playerEntity, nextTurnInput.ReadyForNextTurn));
                break;

            case SimPlayerInputUseItem useItemInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseItem(playerEntity, useItemInput.ItemIndex, useItemInput.UseData));
                break;

            case SimPlayerInputUseInteractable useInteractableInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseInteractable(playerEntity, useInteractableInput.InteractablePosition));
                break;

            case SimPlayerInputSetPawnDoodle setPawnDoodleInput:
            {
                Entity pawn = GetPlayerPawn(playerEntity);
                if(pawn != Entity.Null)
                {
                    EntityManager.SetOrAddComponentData<DoodleId>(pawn, new DoodleId() { Guid = setPawnDoodleInput.DoodleId });
                }
                break;
            }
            case SimPlayerInputEquipItem equipItemInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputEquipItem(playerEntity, equipItemInput.ItemIndex, equipItemInput.ItemEntityPosition));
                break;

            case SimPlayerInputDropItem dropItemInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputDropItem(playerEntity, dropItemInput.ItemIndex));
                break;
        }
    }

    private Entity GetPlayerPawn(Entity playerEntity)
    {
        if (EntityManager.TryGetComponentData(playerEntity, out ControlledEntity controlledEntity))
        {
            Entity pawn = controlledEntity.Value;

            if (EntityManager.Exists(pawn))
            {
                return pawn;
            }
        }

        return Entity.Null;
    }
}

public partial class CommonReads
{
    public static Entity FindPlayerEntity(ISimWorldReadAccessor readAccessor, PersistentId simPlayerId)
    {
        Entity playerEntity = Entity.Null;
        readAccessor.Entities.ForEach((Entity entity, ref PersistentId id, ref PlayerTag playerTag) =>
        {
            if (id == simPlayerId)
            {
                playerEntity = entity;
                return;
            }
        });

        return playerEntity;
    }
}