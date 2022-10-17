using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public static class SaveSystem
{
    static string saveSettingPath = Application.persistentDataPath + "setting.irconfig";
    public static void SaveSetting(AudioManager audioManager, GameManager gameManager)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveSettingPath, FileMode.Create);
        SettingData settingData = new SettingData(audioManager, gameManager);
        binaryFormatter.Serialize(stream, settingData);
        stream.Close();
    }
    public static SettingData LoadSettingData()
    {
        if (File.Exists(saveSettingPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveSettingPath, FileMode.Open);
            SettingData settingData = binaryFormatter.Deserialize(stream) as SettingData;
            stream.Close();
            return settingData;
        }
        else
        {
            Debug.Log("cannot find saved setting file in " + saveSettingPath);
            return null;
        }
    }

}

