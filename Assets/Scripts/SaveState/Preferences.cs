using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class Preferences
{
    private const string PrefDir = "Preferences";
    private const string SaveDir = "Saves";

    private static PreferenceCollection collection = null;

    public static int LastTank
    {
        get { return GetInt(PrefPath("LastTank"), 0); } 
        set { SetInt(PrefPath("LastTank"), value); }
    }

    public static int MaxTanks
    {
        get { return GetInt(PrefPath("MaxTanks"), 6); }
        set { SetInt(PrefPath("MaxTanks"), value); }
    }

    public static float MusicVolume
    {
        get { return GetFloat(PrefPath("MusicVolume"), 1.0f); }
        set { SetFloat(PrefPath("MusicVolume"), value); }
    }

    public static float SfxVolume
    {
        get { return GetFloat(PrefPath("SfxVolume"), 1.0f); }
        set { SetFloat(PrefPath("SfxVolume"), value); }
    }

    public static float UiVolume
    {
        get { return GetFloat(PrefPath("UiVolume"), 1.0f); }
        set { SetFloat(PrefPath("UiVolume"), value); }
    }

    public static bool PumpSoundOn
    {
        get { return GetInt(PrefPath("PumpSoundOn"), 1) != 0; }
        set { SetInt(PrefPath("PumpSoundOn"), value ? 1 : 0); }
    }

    #region "Setters and Getters"

    private static void SetInt(string key, int value)
    {
        GetPreferences().SetInt(key, value);
        SavePreferences();
    }

    private static int GetInt(string key, int defaultValue = 0)
    {
        return GetPreferences().GetInt(key, defaultValue);
    }


    private static void SetFloat(string key, float value)
    {
        GetPreferences().SetFloat(key, value);
        SavePreferences();
    }

    private static float GetFloat(string key, float defaultValue = 0f)
    {
        return GetPreferences().GetFloat(key, defaultValue);
    }


    private static void SetString(string key, string value)
    {
        GetPreferences().SetString(key, value);
        SavePreferences();
    }

    private static string GetString(string key, string defaultValue = null)
    {
        return GetPreferences().GetString(key, defaultValue);
    }

    private static void SavePreferences()
    {
        if (collection == null)
            collection = new PreferenceCollection();

        FileHelper.SaveFile<PreferenceCollection>(collection, PrefPath());
    }

    private static PreferenceCollection GetPreferences()
    {
        if (collection == null)
        {
            if (!File.Exists(PrefPath()))
            {
                collection = new PreferenceCollection();
            }
            else
            {
                collection = FileHelper.LoadFile<PreferenceCollection>(PrefPath());
            }
        }

        return collection;
    }

    private static string PrefPath()
    {
        return Path.Combine(Application.persistentDataPath, "preferences.dat");
    }

    #endregion "Setters and Getters"

    public static string PrefPath(string prefName)
    {
        return $"{PrefDir}/{prefName}";
    }

    public static void SetSave(string saveName, byte[] saveData)
    {
        FileHelper.SaveFile<string>(System.Convert.ToBase64String(saveData), SavePath(saveName));
    }
    public static byte[] GetSave(string saveName)
    {
        if (!File.Exists(SavePath(saveName)))
            return null;

        return System.Convert.FromBase64String(FileHelper.LoadFile<string>(SavePath(saveName)));
    }

    public static string SavePath(string saveName)
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, $"{SaveDir}")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, $"{SaveDir}"));

        return Path.Combine(Application.persistentDataPath, $"{SaveDir}/{saveName}.dat");
    }
}

[Serializable()]
public class PreferenceCollection
{
    public PreferenceCollection()
    {
        Values = new List<KeyedString>();
    }

    public List<KeyedString> Values { get; set; }

    private bool ContainsKey(string key)
    {
        return Values.Any(v => v.Key == key);
    }

    private void SetToList(string key, string value)
    {
        if (ContainsKey(key))
            RemoveFromList(key);

        Values.Add(new KeyedString(key, value));
    }

    private string GetFromList(string key)
    {
        return Values.FirstOrDefault(v => v.Key == key).Value;
    }

    private void RemoveFromList(string key)
    {
        Values.RemoveAll(v => v.Key == key);
    }

    internal float GetFloat(string key, float defaultValue)
    {
        if (ContainsKey(key))
            return float.Parse(GetFromList(key));

        return defaultValue;
    }

    internal int GetInt(string key, int defaultValue)
    {
        if (ContainsKey(key))
            return int.Parse(GetFromList(key));

        return defaultValue;
    }

    internal string GetString(string key, string defaultValue)
    {
        if (ContainsKey(key))
            return GetFromList(key);

        return defaultValue;
    }

    internal void SetFloat(string key, float value)
    {
        SetToList(key, value.ToString());
    }

    internal void SetInt(string key, int value)
    {
        SetToList(key, value.ToString());
    }

    internal void SetString(string key, string value)
    {
        SetToList(key, value);
    }
}

[Serializable]
public class KeyedString
{
    public KeyedString()
    {

    }
    public KeyedString(string key, string value) : this()
    {
        Key = key;
        Value = value;
    }
    public string Key { get; set; }
    public string Value { get; set; }
}
public static class FileHelper
{
    public static T LoadFile<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {

            if (!File.Exists(path))
                throw new FileNotFoundException($"LoadFile could not locate the file {path}.");


            stream.Position = 0;
            stream.Flush();

            return (T)serializer.Deserialize(stream);
        }

    }

    public static void SaveFile<T>(T value, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {

            stream.Position = 0;
            stream.SetLength(0);
            stream.Flush();

            serializer.Serialize(stream, value);

            stream.Flush();
            stream.Close();
        }
    }
}