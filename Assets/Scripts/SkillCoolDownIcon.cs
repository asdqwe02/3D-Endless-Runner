using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillCoolDownIcon : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TMP_Text cooldownText;
    public float cooldownTime;
    public float cooldownTimer;
    bool cooldown;

    void Awake()
    {
        cooldown = false;
        cooldownText.gameObject.SetActive(false);
        cooldownImage.fillAmount = 0f;
        gameObject.SetActive(false);
    }
    private void Start()
    {
        PlayerController.instance.SkillButtonPressed += SetUpCoolDownEffect;
    }
    // Update is called once per frame
    void Update()
    {
        if (cooldown)
        {
            ApplyCoolDownEffect();
        }
    }
    public void ApplyCoolDownEffect()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            cooldownText.gameObject.SetActive(false);
            cooldownImage.fillAmount = 0f;
            cooldown = false;
        }
        else
        {
            cooldownText.text = cooldownTimer.ToString("F1");
            cooldownImage.fillAmount = cooldownTimer / cooldownTime;
        }
    }
    public void SetUpCoolDownEffect(object sender, float time)
    {
        cooldown = true;
        cooldownText.gameObject.SetActive(true);
        cooldownImage.fillAmount = 1f;
        cooldownTime = time;
        cooldownTimer = cooldownTime;
    }

}
