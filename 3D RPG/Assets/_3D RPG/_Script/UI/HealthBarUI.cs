using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;

    //是否总是可见
    public bool alwaysVisable;

    //可见时间
    public float visableTime;
    //剩余可见时间
    private float timeLeft;

    Image healthSlider;
    Transform UIbar;
    Transform cam;
    CharactersStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharactersStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;

        //找到血条的Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //以渲染模式为世界空间的Canvas
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;

                //找到血条的Image
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisable);
            }
        }
    }

    //更新血条
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //生命值小于0时，不显示血条
        if (currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }

        //受击时总是显示血条
        UIbar.gameObject.SetActive(true);

        //开始计时
        timeLeft = visableTime;

        //计算血条的比例
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            //血条始终面向摄像机
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            //计时
            if(timeLeft <= 0 && !alwaysVisable)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
