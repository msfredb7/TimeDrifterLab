﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGridTransformComponent : SimTransformComponent
{
    public SimTileId tileId => SimTileId.FromWorldPosition(worldPosition);
}