using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionSwap : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 2;
    const int RANGE = 3;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description()
            {
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = RANGE
            });
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value;

            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<ControllableTag>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                if (accessor.HasComponent<FixTranslation>(entity))
                {
                    fix3 targetPos = accessor.GetComponentData<FixTranslation>(entity).Value;
                    accessor.SetComponentData(context.InstigatorPawn, new FixTranslation() { Value = targetPos });
                    accessor.SetComponentData(entity, new FixTranslation() { Value = instigatorPos });
                }
            }
        }
    }
}
