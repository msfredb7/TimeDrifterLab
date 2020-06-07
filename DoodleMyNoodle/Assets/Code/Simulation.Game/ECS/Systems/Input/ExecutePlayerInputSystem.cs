﻿using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class ExecutePlayerInputSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimPlayerInput playerInput)
            {
                Entity playerEntity = FindPlayerEntity(playerInput.SimPlayerId);
                ExecutePlayerInput(playerInput, playerEntity);
            }
        }
    }

    private Entity FindPlayerEntity(PersistentId simPlayerId)
    {
        Entity playerEntity = Entity.Null;
        Entities.ForEach((Entity entity, ref PersistentId id, ref PlayerTag playerTag) =>
        {
            if (id == simPlayerId)
            {
                playerEntity = entity;
                return;
            }
        });

        return playerEntity;
    }

    private void ExecutePlayerInput(SimPlayerInput input, Entity playerEntity)
    {
        // fbessette: For now, we simply do a switch. 
        //            In the future, we'll probably want to implement something dynamic instead

        // THIS IS MAINLY FOR DEBUG

        switch (input)
        {
            // temporary
            /* case SimInputKeycode keycodeInput:
            {
                Entity pawn = GetPlayerPawn(playerEntity);

                if (pawn != Entity.Null)
                {
                    if (EntityManager.TryGetComponentData(pawn, out FixTranslation pawnPos))
                    {
                        if (keycodeInput.state == SimInputKeycode.State.Pressed)
                        {
                            switch (keycodeInput.keyCode)
                            {
                                // Damage Health Debug 
                                case UnityEngine.KeyCode.M:
                                    CommonWrites.SetStatInt(Accessor, pawn, new Health()
                                    {
                                        Value = EntityManager.GetComponentData<Health>(pawn).Value - 1
                                    });
                                    break;
                                // Consume 1 Action Debug 
                                case UnityEngine.KeyCode.N:
                                    CommonWrites.SetStatInt(Accessor, pawn, new ActionPoints()
                                    {
                                        Value = EntityManager.GetComponentData<ActionPoints>(pawn).Value - 1
                                    });
                                    break;
                            }
                        }
                    }

                    if (keycodeInput.keyCode == UnityEngine.KeyCode.X && keycodeInput.state == SimInputKeycode.State.Pressed)
                    {
                        Entities.ForEach((Entity aiController, ref AITag aiTag, ref ControlledEntity p) =>
                        {
                            EntityManager.DestroyEntity(p.Value);
                        });

                    }

                    if (keycodeInput.keyCode == UnityEngine.KeyCode.T && keycodeInput.state == SimInputKeycode.State.Pressed
                        && EntityManager.HasComponent<FixTranslation>(pawn))
                    {
                        fix2 newPosition = World.Random().NextFixVector2(
                            min: fix2(-5, -5),
                            max: fix2(5, 5));

                        EntityManager.SetComponentData(pawn, new FixTranslation() { Value = fix3(newPosition, 0) });
                    }
                }
                break;
            }*/
            case SimPlayerInputUseItem ItemUsedInput:
                ExecutePawnControllerInputSystem pawnControllerInputSystem = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseItem(playerEntity, ItemUsedInput.ItemIndex, ItemUsedInput.UseData));
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