﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnlineInterface : IDisposable
{
    const bool log = false;

    public event Action onTerminate;
    public SessionInterface SessionInterface { get; protected set; }

    public abstract bool IsServerType { get; }
    public bool isClientType => !IsServerType;

    public OnlineInterface(NetworkInterface network)
    {
        _network = network;

        _network.OnDisconnectedFromSession += OnDisconnectFromSession;

        if (log)
#pragma warning disable CS0162 // Unreachable code detected
            DebugService.Log("Online interface created");
#pragma warning restore CS0162 // Unreachable code detected
    }

    public void Update()
    {
        SessionInterface?.Update();
    }

    protected virtual void OnDisconnectFromSession()
    {
        SessionInterface?.Dispose();
        SessionInterface = null;
    }

    public virtual void Dispose()
    {
        SessionInterface?.Dispose();
        _network.OnDisconnectedFromSession -= OnDisconnectFromSession;

        if (log)
#pragma warning disable CS0162 // Unreachable code detected
            DebugService.Log("Online interface terminating");
#pragma warning restore CS0162 // Unreachable code detected

        onTerminate?.Invoke();
    }

    protected NetworkInterface _network;
}