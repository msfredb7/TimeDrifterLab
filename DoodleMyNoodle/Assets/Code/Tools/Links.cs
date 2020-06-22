﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class Links : MonoBehaviour
{
    [MenuItem("Links/Documentation/Project Dependencies")]
    static void OpenProjectDependenciesDocumentation()
    {
        Process.Start("https://github.com/CCC-Development/DoodleMyNoodle/tree/master/Documentation/ProjectDependencies/ProjectDependencies.md");
    }

    [MenuItem("Links/Presistent Data Path #&p")]
    public static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
        else
        {
            Log.Warning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }
}
