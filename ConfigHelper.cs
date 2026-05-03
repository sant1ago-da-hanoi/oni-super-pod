using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;

public class ConfigHelper<T>
{
    private static T _instance;
    private static bool configFileUpdated;
    private static FileSystemWatcher fileSystemWatcher;

    public static T GetConfig(string fileName)
    {
        if (_instance != null && !configFileUpdated)
            return _instance;

        T val = (T)AccessTools.CreateInstance(typeof(T));
        string configDir = Path.Combine(GetModDirPath(), "Config");
        string configPath = Path.Combine(configDir, fileName);

        if (!File.Exists(configPath))
        {
            Debug.Log("[SuperPOD] Config file not found: " + configPath);
            return val;
        }

        if (fileSystemWatcher == null)
            AddConfigFileWatcher(configDir, fileName);

        Debug.Log("[SuperPOD] Loading config: " + configPath);
        configFileUpdated = false;

        var entries = new Dictionary<string, string>();
        try
        {
            using var reader = new StreamReader(configPath, Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();
                if (line.StartsWith("#") || line.StartsWith(";") || !line.Contains("="))
                    continue;

                Match match = Regex.Match(line, @"^(?<key>[a-zA-Z_][a-zA-Z_\d.]*)\s*=\s*(?<val>.+)$");
                if (match.Success)
                {
                    string key = match.Groups["key"].Value.Trim();
                    string value = match.Groups["val"].Value.Trim();
                    entries[key] = value;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[SuperPOD] Error reading config: " + ex);
            return val;
        }

        Traverse traverse = Traverse.Create((object)val);
        foreach (string propertyName in AccessTools.GetPropertyNames(typeof(T)))
        {
            if (!entries.ContainsKey(propertyName))
                continue;

            Type valueType = traverse.Property(propertyName).GetValueType();
            try
            {
                if (valueType == typeof(int))
                    traverse.Property(propertyName).SetValue(int.Parse(entries[propertyName]));
                else if (valueType == typeof(long))
                    traverse.Property(propertyName).SetValue(long.Parse(entries[propertyName]));
                else if (valueType == typeof(float))
                    traverse.Property(propertyName).SetValue(float.Parse(entries[propertyName]));
                else if (valueType == typeof(double))
                    traverse.Property(propertyName).SetValue(double.Parse(entries[propertyName]));
                else if (valueType == typeof(bool))
                    traverse.Property(propertyName).SetValue(bool.Parse(entries[propertyName]));
                else
                    traverse.Property(propertyName).SetValue(entries[propertyName]);
            }
            catch (Exception)
            {
                Debug.Log("[SuperPOD] Invalid value for [" + propertyName + "] in config: " + configPath);
            }
        }

        _instance = val;
        return val;
    }

    private static void AddConfigFileWatcher(string dirPath, string fileName)
    {
        fileSystemWatcher = new FileSystemWatcher(dirPath);
        fileSystemWatcher.Changed += (sender, e) =>
        {
            if (fileName.Equals(Path.GetFileName(e.Name)))
            {
                configFileUpdated = true;
                Debug.Log("[SuperPOD] Config file changed: " + e.FullPath);
            }
        };
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private static string GetModDirPath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
