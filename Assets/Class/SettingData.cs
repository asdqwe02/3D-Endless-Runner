using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
[System.Serializable]
public class SettingData 
{
    public float masterVolume, musicVolume;
    public int screenWidth, screenHeight;
    public bool fullScreen, bloom;

    public SettingData(AudioManager audioManager, GameManager gameManager)
    {
        masterVolume = audioManager.MasterVolume;
        musicVolume = audioManager.SountrackVolume;
        screenWidth = gameManager.screenWidth;
        screenHeight = gameManager.screenHeight;
        fullScreen = gameManager.fullScreen;
        bloom = gameManager.bloom;
    }
}
