using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Static class responsible for managing save data.
/// </summary>
public static class SaveDataManager
{
    /// <summary>
    /// Gets a save data from the passes slot.
    /// </summary>
    /// <param name="slot">Slot to get the save data from.</param>
    /// <returns>The save data read from the file.</returns>
    public static SaveData LoadSaveData(int slot)
    {
        SaveData returnVal = null;
        string written;

        Directory.CreateDirectory(Application.persistentDataPath + "/Save data");

        try
        {
            using(StreamReader reader = new StreamReader(Application.persistentDataPath + "/Save data/Save_Data_Slot" + slot + ".json"))
            {
                written = reader.ReadToEnd();
                returnVal = JsonUtility.FromJson<SaveData>(written);
                reader.Close();
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning("File not found. Returning null.");
            return null;
        }
        

        return returnVal;
    }

    /// <summary>
    /// Tries the saved data into a file.
    /// </summary>
    /// <param name="data">The data to be saved.</param>
    /// <param name="slot">The slot to be saved into.</param>
    /// <returns>True if it was possible to save the data, false otherwise.</returns>
    public static bool TrySaveData(SaveData data, int slot)
    {
        try
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save data/");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/Save data/Save_Data_Slot" + slot + ".json"))
        {
            writer.Write(JsonUtility.ToJson(data, true));
            writer.Close();
            return true;
        }

        return false;
    }

    public static SaveData LoadDummyData()
    {
        var saveToReturn = LoadSaveData(66);
        Debug.Log(saveToReturn);
        return saveToReturn;
    }
    
    public static void WriteDummyData()
    {
        SaveData toSave = new SaveData();
        toSave.slotNumber = 66;
        Debug.Log("Trying to save data. Result:" + TrySaveData(toSave, toSave.slotNumber));
    }
}