﻿using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnDuration : IComponentData
{
    public fix DurationAI;
    public fix DurationPlayer;
}