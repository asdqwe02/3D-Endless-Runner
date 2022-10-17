using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingPanelNavController : MonoBehaviour
{
    [SerializeField] List<Transform> _panelList;
    [SerializeField] Button _leftButton, _rightButton;
    private int _panelIndex, _previousPanelIndex;


    private void Awake()
    {
       
        _panelIndex = 0;
        _previousPanelIndex = _panelIndex;
        _leftButton.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
            _panelIndex--;
            IndexBoundCheck();
            ChangePanel();
        });
        _rightButton.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
            _panelIndex++;
            IndexBoundCheck();
            ChangePanel();
        });
        ChangePanel();


    }
    public void ChangePanel()
    {
        _panelList[_previousPanelIndex].gameObject.SetActive(false);
        _panelList[_panelIndex].gameObject.SetActive(true);
        _previousPanelIndex = _panelIndex;

    }
    public void IndexBoundCheck()
    {
        if (_panelIndex >= _panelList.Count)
        {
            _panelIndex = 0;
        }
        else if (_panelIndex < 0)
        {
            _panelIndex = _panelList.Count - 1;
        }
    }
}

