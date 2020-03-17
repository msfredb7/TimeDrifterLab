﻿using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    [UpdateBefore(typeof(TickSimulationSystem))]
    public class SendSimulationSyncSystem : ComponentSystem
    {
        [ConfigVar("sim.pause_while_join", "true", description: "Should the simulation be paused while players are joining the game?")]
        static ConfigVar s_pauseSimulationWhilePlayersAreJoining;

        private SessionServerInterface _session;
        private List<CoroutineOperation> _ongoingOperations = new List<CoroutineOperation>();
        private SimulationWorldSystem _simWorldSystem;
        private TickSimulationSystem _tickSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _session = OnlineService.ServerInterface.SessionServerInterface;
            _session.RegisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);

            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnDestroy()
        {
            _session.UnregisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);

            foreach (var item in _ongoingOperations)
            {
                if (item.IsRunning)
                    item.TerminateWithFailure();
            }

            base.OnDestroy();
        }

        private void OnSimSyncRequest(NetMessageRequestSimSync requestSync, INetworkInterfaceConnection source)
        {
            LaunchSyncForClient(source);
        }

        protected override void OnUpdate()
        {
            if(_ongoingOperations.Count > 0)
            {
                for (int i = _ongoingOperations.Count - 1; i >= 0; i--)
                {
                    if (!_ongoingOperations[i].IsRunning)
                        _ongoingOperations.RemoveAt(i);
                }

                if(_ongoingOperations.Count == 0)
                {
                    _tickSystem.UnpauseSimulation(key: "PlayerJoining");
                }
            }
        }


        SimulationSyncFromTransferServerOperation LaunchSyncForClient(INetworkInterfaceConnection clientConnection)
        {
            DebugService.Log($"Starting new sync...");

            var newOp = new SimulationSyncFromTransferServerOperation(_session, clientConnection, _simWorldSystem.SimulationWorld);

            newOp.OnFailCallback = (op) =>
            {
                DebugService.Log($"Sync failed. {op.Message}");
            };

            newOp.OnSucceedCallback = (op) =>
            {
                DebugService.Log($"Sync complete. {op.Message}");
            };

            newOp.Execute();

            _ongoingOperations.Add(newOp);

            if (s_pauseSimulationWhilePlayersAreJoining.BoolValue)
            {
                _tickSystem.PauseSimulation(key: "PlayerJoining");
            }

            return newOp;
        }
    }
}