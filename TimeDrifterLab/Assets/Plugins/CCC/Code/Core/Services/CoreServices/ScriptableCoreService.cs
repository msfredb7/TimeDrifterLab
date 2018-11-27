﻿using System;
using UnityEngine;

public abstract class ScriptableCoreService : ScriptableObject, ICoreService
{
    public abstract void Initialize(Action onComplete);

    public virtual ICoreService ProvideOfficialInstance() // Can be overriden
    {
        return this;
    }
}
