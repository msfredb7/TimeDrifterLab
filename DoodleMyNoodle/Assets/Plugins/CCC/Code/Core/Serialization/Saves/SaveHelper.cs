﻿using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using UnityEngineX;

public static class SaveHelper
{
    static public void InstantSave(string path, object graph)
    {
        InstantSave(path, graph, new BinaryFormatter());
    }
    static public void InstantSave(string path, object graph, IFormatter formatter)
    {
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        formatter.Serialize(file, graph);
        file.Close();
    }

    static public object InstantLoad(string path)
    {
        return InstantLoad(path, new BinaryFormatter());
    }
    static public object InstantLoad(string path, IFormatter formatter)
    {
        if (!FileExists(path))
            return null;

        object obj = null;
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            obj = formatter.Deserialize(file);
        }
        catch (Exception e)
        {
            Log.Error("Failed to deserialize the following file:\n" + path + "\n\nError:\n" + e.Message);
        }

        file.Close();

        return obj;
    }

    static public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    static public bool DeleteFile(string path)
    {
        if (!FileExists(path))
            return false;

        File.Delete(path);
        return true;
    }
}
