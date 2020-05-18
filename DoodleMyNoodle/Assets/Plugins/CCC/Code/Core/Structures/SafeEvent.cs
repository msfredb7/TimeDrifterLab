﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngineX;

public abstract class SafeEventBase<ActionClass>
{
    protected List<ActionClass> actions = new List<ActionClass>();
    protected int currentActionExecutionIndex = -1;

    protected bool IsExecutingActions => currentActionExecutionIndex > 0;

    public void AddAction(ActionClass action)
    {
        actions.Add(action);
    }
    public bool RemoveAction(ActionClass action)
    {
        if (IsExecutingActions)
        {
            int index = actions.IndexOf(action);
            if (index > 0)
            {
                actions.RemoveAt(index);
                if (index <= currentActionExecutionIndex)
                {
                    currentActionExecutionIndex--;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return actions.Remove(action);
        }
    }

    protected void BeginActionExecution()
    {
        currentActionExecutionIndex = 0;
    }
    protected void EndActionExecution()
    {
        currentActionExecutionIndex = -1;
    }
}

public class SafeEvent : SafeEventBase<Action>
{
    public static SafeEvent operator +(SafeEvent a, Action b)
    {
        a.AddAction(b);
        return a;
    }
    public static SafeEvent operator -(SafeEvent a, Action b)
    {
        a.RemoveAction(b);
        return a;
    }
    public void SafeInvoke(LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchException(logMode);
        }
        EndActionExecution();
    }
    public void SafeInvokeInEditor(LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchExceptionInEditor(logMode);
        }
        EndActionExecution();
    }
}

public class SafeEvent<T> : SafeEventBase<Action<T>>
{
    public static SafeEvent<T> operator +(SafeEvent<T> a, Action<T> b)
    {
        a.AddAction(b);
        return a;
    }
    public static SafeEvent<T> operator -(SafeEvent<T> a, Action<T> b)
    {
        a.RemoveAction(b);
        return a;
    }
    public void SafeInvoke(T value, LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchException(value, logMode);
        }
        EndActionExecution();
    }
    public void SafeInvokeInEditor(T value, LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchExceptionInEditor(value, logMode);
        }
        EndActionExecution();
    }
}

public class SafeEvent<T1, T2> : SafeEventBase<Action<T1, T2>>
{
    public static SafeEvent<T1, T2> operator +(SafeEvent<T1, T2> a, Action<T1, T2> b)
    {
        a.AddAction(b);
        return a;
    }
    public static SafeEvent<T1, T2> operator -(SafeEvent<T1, T2> a, Action<T1, T2> b)
    {
        a.RemoveAction(b);
        return a;
    }
    public void SafeInvoke(T1 value1, T2 value2, LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchException(value1, value2, logMode);
        }
        EndActionExecution();
    }
    public void SafeInvokeInEditor(T1 value1, T2 value2, LogMode logMode = LogMode.Error)
    {
        BeginActionExecution();
        for (; currentActionExecutionIndex < actions.Count; currentActionExecutionIndex++)
        {
            actions[currentActionExecutionIndex].InvokeCatchExceptionInEditor(value1, value2, logMode);
        }
        EndActionExecution();
    }
}