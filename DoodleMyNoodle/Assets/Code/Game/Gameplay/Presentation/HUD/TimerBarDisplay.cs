﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class TimerBarDisplay : GamePresentationBehaviour
{
    public Slider TimerBar;
    public int TimeToStartShowing = 10;

    public GameObject BarContainer;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetSingleton(out TurnTimerSingletonComponent turnTimer) 
            && SimWorld.TryGetSingleton(out TurnDurationSingletonComponent turnDuration)
            && SimWorld.TryGetSingleton(out TurnCurrentTeamSingletonComponent turnTeam))
        {
            if (turnTimer.Value <= TimeToStartShowing)
            {
                BarContainer.SetActive(true);

                Color color;
                fix teamTurnDuration;
                switch (turnTeam.Value)
                {
                    case (int)TurnSystemSetting.Team.AI:
                        color = Color.red;
                        teamTurnDuration = turnDuration.DurationAI;
                        break;

                    default:
                    case (int)TurnSystemSetting.Team.Players:
                        color = Color.blue;
                        teamTurnDuration = turnDuration.DurationPlayer;
                        break;
                }

                TimerBar.fillRect.GetComponent<Image>().color = Color.blue;
                TimerBar.value = (float)(turnTimer.Value / fixMath.min(teamTurnDuration, TimeToStartShowing));
            }
            else
            {
                BarContainer.SetActive(false);
            }
        }
    }
}
