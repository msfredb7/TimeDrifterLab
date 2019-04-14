﻿using System;
using System.Collections.Generic;

public class PlayerRepertoireClient : PlayerRepertoireSystem
{
    public static new PlayerRepertoireClient instance => (PlayerRepertoireClient)GameSystem<PlayerRepertoireSystem>.instance;

    SessionClientInterface _clientSession;

    bool _localPlayerIdAssigned = false;
    bool _playerListSyncReceived = false;

    public override bool isSystemReady => _localPlayerIdAssigned && _playerListSyncReceived;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
    {
        if(_clientSession != null && _clientSession.serverConnection != null && _clientSession.serverConnection.Id == connection.Id)
        {
            return GetServerPlayerInfo();
        }
        else
        {
            return null;
        }
    }

    protected override void OnBindedToSession()
    {
        _clientSession = (SessionClientInterface)_sessionInterface;
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerIdAssignment>(OnMsg_PlayerIdAssignement);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerRepertoireSync>(OnMsg_NetMessagePlayerRepertoireSync);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerJoined>(OnMsg_NetMessagePlayerJoined);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerLeft>(OnMsg_NetMessagePlayerLeft);
    }

    protected override void OnUnbindedFromSession()
    {
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerIdAssignment>(OnMsg_PlayerIdAssignement);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerRepertoireSync>(OnMsg_NetMessagePlayerRepertoireSync);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerJoined>(OnMsg_NetMessagePlayerJoined);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerLeft>(OnMsg_NetMessagePlayerLeft);
        _clientSession = null;
    }

    protected override void Internal_OnGameReady()
    {
        _localPlayerInfo.isServer = false;
        _localPlayerInfo.playerId = PlayerId.invalid;

        // Say hello to server ! (this should initiate the process of being added to valid players)
        NetMessageClientHello helloMessage = new NetMessageClientHello()
        {
            playerName = _localPlayerInfo.playerName
        };
        _clientSession.SendNetMessageToServer(helloMessage);
        DebugService.Log("[PlayerRepertoireClient] Hello sent");
    }

    void OnMsg_PlayerIdAssignement(NetMessagePlayerIdAssignment message, INetworkInterfaceConnection source)
    {
        DebugService.Log("[PlayerRepertoireClient] OnMsg_PlayerIdAssignement");
        _localPlayerInfo.playerId = message.playerId;

        _localPlayerIdAssigned = true;
    }

    void OnMsg_NetMessagePlayerRepertoireSync(NetMessagePlayerRepertoireSync message, INetworkInterfaceConnection source)
    {
        DebugService.Log("[PlayerRepertoireClient] OnMsg_NetMessagePlayerRepertoireSync");
        _players.Clear();
        foreach (var playerInfo in message.players)
        {
            _players.Add(new PlayerInfo(playerInfo));
        }

        _playerListSyncReceived = true;
    }

    void OnMsg_NetMessagePlayerJoined(NetMessagePlayerJoined message, INetworkInterfaceConnection source)
    {
        DebugService.Log("[PlayerRepertoireClient] OnMsg_NetMessagePlayerJoined");
        _players.Add(new PlayerInfo(message.playerInfo));
    }

    void OnMsg_NetMessagePlayerLeft(NetMessagePlayerLeft message, INetworkInterfaceConnection source)
    {
        DebugService.Log("[PlayerRepertoireClient] OnMsg_NetMessagePlayerLeft");
        _players.RemoveFirst((p) => p.playerId == message.playerId);
    }
}