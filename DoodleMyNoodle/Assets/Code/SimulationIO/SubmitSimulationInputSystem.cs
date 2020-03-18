﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;


namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SubmitSimulationInputSystem : ComponentSystem
    {
        private SessionInterface GetSession() => OnlineService.OnlineInterface?.SessionInterface;

        protected override void OnCreate()
        {
            base.OnCreate();

            World.GetOrCreateSystem<SimulationWorldSystem>().SimWorldAccessor.SubmitSystem = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            World.GetOrCreateSystem<SimulationWorldSystem>().SimWorldAccessor.SubmitSystem = null;
        }

        protected override void OnUpdate() { }

        public InputSubmissionId SubmitInput(SimInput input)
        {
            if (input == null)
            {
                DebugService.LogError("Trying to submit a null input");
                return InputSubmissionId.Invalid;
            }

            var session = GetSession();
            if (session != null && session is SessionClientInterface clientSession)
            {
                // CLIENT
                var syncSystem = World.GetExistingSystem<ReceiveSimulationSyncSystem>();
                if (syncSystem != null && syncSystem.IsSynchronizing)
                {
                    DebugService.Log("Discarding input since we are syncing to the simulation");
                    return InputSubmissionId.Invalid;
                }

                var submissionId = InputSubmissionId.Generate();
                clientSession.SendNetMessageToServer(new NetMessageInputSubmission()
                {
                    submissionId = submissionId,
                    input = input
                });
                return submissionId;
            }
            else
            {
                // SERVER AND LOCAL
                var submissionId = InputSubmissionId.Generate();
                World.GetOrCreateSystem<ConstructSimulationTickSystem>()
                    .SubmitInputInternal(
                    input: input,
                    instigatorConnection: null, // local connection => null
                    submissionId: submissionId);
                return submissionId;
            }
        }
    }
}