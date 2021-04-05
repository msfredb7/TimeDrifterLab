using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;

public class GameActionDashAttack : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionRangeData),
        typeof(GameActionDamageData),
        typeof(GameActionAPCostData)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Item).Value)
            {
                IncludeSelf = false,
                MustBeReachable = true
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
            int moveRange = accessor.GetComponentData<GameActionRangeData>(context.Item).Value;

            NativeList<int2> _path = new NativeList<int2>(Allocator.Temp);
            if (!Pathfinding.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_LENGTH, _path))
            {
                LogGameActionInfo(context, $"Discarding: cannot find navigable path from { instigatorTile} to { paramTile.Tile}.");
                return false;
            }

            // Get the last reachable point considering the user's AP
            int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWithinCost(_path.AsArray().Slice(), moveRange);

            // Remove unreachable points
            _path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            int damage = accessor.GetComponentData<GameActionDamageData>(context.Item).Value;
            NativeList<Entity> entityToDamage = new NativeList<Entity>(Allocator.Temp);
            for (int i = 0; i < _path.Length; i++)
            {
                int2 pos = _path[i];
                if (!pos.Equals(instigatorTile))
                {
                    CommonReads.FindTileActorsWithComponent<Health>(accessor, CommonReads.GetTileEntity(accessor, pos), entityToDamage);
                }
            }

            // set destination
            accessor.SetOrAddComponentData(context.InstigatorPawn, new Destination() { Value = Helpers.GetTileCenter(_path[_path.Length - 1]) });

            foreach (Entity entity in entityToDamage)
            {
                CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, damage);
            }

            return true;
        }

        return false;
    }
}