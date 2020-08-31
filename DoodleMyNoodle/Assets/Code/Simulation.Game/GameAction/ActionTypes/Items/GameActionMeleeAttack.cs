﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;

public class GameActionMeleeAttack : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,
                RequiresAttackableEntity = true,
            });
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return 0;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;

            // melee attack has a range of RANGE
            if (lengthmanhattan(paramTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                LogGameActionInfo(context, $"Melee attack at {paramTile.Tile} out of range. Ignoring.");
                return;
            }

            // Cooldown
            if (accessor.TryGetComponentData(context.Entity, out ItemTimeCooldownData itemTimeCooldownData))
            {
                accessor.SetOrAddComponentData(context.Entity, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
            }
            else if (accessor.TryGetComponentData(context.Entity, out ItemTurnCooldownData itemTurnCooldownData))
            {
                accessor.SetOrAddComponentData(context.Entity, new ItemCooldownTurnCounter() { Value = itemTurnCooldownData.Value });
            }

            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, accessor.GetComponentData<ItemDamageData>(context.Entity).Value);
            }
        }
    }
}
