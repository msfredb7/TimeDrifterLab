﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseX
{
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        List<T> assets = new List<T>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static List<GameObject> FindPrefabsAssets()
    {
        string[] guids = AssetDatabase.FindAssets($"t:prefab");
        List<GameObject> assets = new List<GameObject>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static void FindPrefabsAssets(out List<KeyValuePair<string, GameObject>> result)
    {
        string[] guids = AssetDatabase.FindAssets($"t:prefab");
        result = new List<KeyValuePair<string, GameObject>>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset != null)
            {
                result.Add(new KeyValuePair<string, GameObject>(guids[i], asset));
            }
        }
    }

    // DOES NOT DEEP SEARCH INSIDE THE PREFAB
    public static List<GameObject> FindPrefabsAssetsWithComponent<T>() where T : UnityEngine.Component
    {
        List<GameObject> prefabAssets = FindPrefabsAssets();

        for (int i = 0; i < prefabAssets.Count; i++)
        {
            if(prefabAssets[i].GetComponent<T>() == null)
            {
                prefabAssets.RemoveWithLastSwapAt(i);
                i--;
            }
        }

        return prefabAssets;
    }

    // DOES NOT DEEP SEARCH INSIDE THE PREFAB
    public static void FindPrefabsAssetsWithComponent<T>(out List<KeyValuePair<string, GameObject>> result) where T : UnityEngine.Component
    {
        FindPrefabsAssets(out result);

        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].Value.GetComponent<T>() == null)
            {
                result.RemoveWithLastSwapAt(i);
                i--;
            }
        }
    }


    public static T GetOrCreateDataAsset<T>(string path) where T : ScriptableObject, new()
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset == null)
        {
            AssetDatabaseX.CreateFolderFromPath(path.Remove(path.LastIndexOf('/')));
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
            asset = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return asset;
    }

    public static void CreateFolderFromPath(string path)
    {
        CreateFolderFromPath(path, out string dummy);
    }
    public static void CreateFolderFromPath(string path, out string folderGuid)
    {
        folderGuid = "";
        if (AssetDatabase.IsValidFolder(path) == false)
        {
            string[] steps = path.Split('/');


            string currentPath = steps[0];
            string parentPath = steps[0];
            if (currentPath != "Assets")
            {
                Debug.LogError("the path should start with Assets/");
                return;
            }

            for (int i = 1; i < steps.Length; i++) // NB: we start at i=1 to skip the 'Asset/'
            {
                currentPath = $"{currentPath}/{steps[i]}";

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    Debug.Log($"Creating folder {currentPath}");
                    folderGuid = AssetDatabase.CreateFolder(parentPath, steps[i]);
                }

                parentPath = currentPath;
            }
        }
    }

    public static string GetFileNameFromPath(string path)
    {
        return path.Substring(path.LastIndexOf('/') + 1);
    }

    public static string GetAssetNameFromPath(string path)
    {
        string fileName = GetFileNameFromPath(path);
        return fileName.Remove(fileName.LastIndexOf('.'));
    }
}