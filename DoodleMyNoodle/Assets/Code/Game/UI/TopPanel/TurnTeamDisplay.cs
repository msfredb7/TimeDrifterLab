﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnTeamDisplay : SingletonEntityDataDisplay<TurnCurrentTeam>
{
    [System.Serializable]
    public struct TeamTurnText
    {
        public TeamAuth.DesignerFriendlyTeam Team;
        public string Text;
    }

    public TextMeshProUGUI TextDisplay;

    public List<TeamTurnText> TeamTurnTexts = new List<TeamTurnText>();

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();
        //if (SimWorld.HasSingleton<TurnCurrentTeam>())
        //{
        //    SetCurrentTeam((TeamAuth.DesignerFriendlyTeam)SimWorld.GetSingleton<TurnCurrentTeam>().Value);
        //}

        if(_singletonData != Entity.Null) 
        {

        }
    }

    private void SetCurrentTeam(TeamAuth.DesignerFriendlyTeam team)
    {
        for (int i = 0; i < TeamTurnTexts.Count; i++)
        {
            if(TeamTurnTexts[i].Team == team)
            {
                TextDisplay.text = TeamTurnTexts[i].Text;
            }
        }
    }
}