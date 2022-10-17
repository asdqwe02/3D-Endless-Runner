using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    Slider progressSlider;
    TextMeshProUGUI progressText;
    private void Awake()
    {
        if (loadingScreen != null)
        {
            progressSlider = loadingScreen.GetComponentInChildren<Slider>();
            progressText = progressSlider.GetComponentInChildren<TextMeshProUGUI>();

        }
    }
    public void LoadLevel(int index)
    {
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
        StartCoroutine(LoadAsync(index));
    }
    public IEnumerator LoadAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false; // debug purpose
        loadingScreen.SetActive(true);
        float progress;
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / .9f);
            progressSlider.value = progress;
            progressText.text = (progress * 100f).ToString("#.##") + "%";
            if (operation.progress >= 0.9f)
            {

                progressText.text = "99%";
                progressSlider.value = .99f;
                yield return new WaitForSeconds(0.75f);
                operation.allowSceneActivation = true; 
            }
            yield return null;
        }
    }
}
