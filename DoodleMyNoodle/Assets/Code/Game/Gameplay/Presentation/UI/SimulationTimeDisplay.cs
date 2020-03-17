﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationTimeDisplay : GameMonoBehaviour
{
    public Text Text;
    public string Prefix = "SimTime: ";

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        Text.text = $"{Prefix}{GameMonoBehaviourHelpers.SimulationWorld.Time.ElapsedTime:F2}";
    }
}
