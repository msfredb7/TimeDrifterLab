﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public Text CurrentTeamName;
    public Text CurrentTime;

    private AutoResetDirtyValue<string> _timer;

    void Update()
    {
        if(SimTurnManager.Instance != null)
        {
            // Timer
            int currentTime = (int)SimTurnManager.Instance.TurnRemainingTime;
            _timer.SetValue(currentTime.ToString());
            CurrentTime.text = _timer.Value;

            if (currentTime <= 3)
            {
                CurrentTime.color = Color.red;
            }
            else
            {
                CurrentTime.color = Color.black;
            }

            if (_timer.IsDirty)
            {
                // fade text shortly
            }

            // Team Text
            Team currentTeam = SimTurnManager.Instance.CurrentTeam;
            CurrentTeamName.text = currentTeam.ToString();
            switch (currentTeam)
            {
                case Team.Player:
                    CurrentTeamName.color = Color.green;
                    break;
                case Team.AI:
                    CurrentTeamName.color = Color.red;
                    break;
                default:
                    break;
            }
        }
    }
}