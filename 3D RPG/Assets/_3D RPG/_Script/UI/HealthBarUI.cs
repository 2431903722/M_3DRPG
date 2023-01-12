using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;

    //�Ƿ����ǿɼ�
    public bool alwaysVisable;

    //�ɼ�ʱ��
    public float visableTime;
    //ʣ��ɼ�ʱ��
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

        //�ҵ�Ѫ����Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //����ȾģʽΪ����ռ��Canvas
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;

                //�ҵ�Ѫ����Image
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisable);
            }
        }
    }

    //����Ѫ��
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //����ֵС��0ʱ������ʾѪ��
        if (currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }

        //�ܻ�ʱ������ʾѪ��
        UIbar.gameObject.SetActive(true);

        //��ʼ��ʱ
        timeLeft = visableTime;

        //����Ѫ���ı���
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            //Ѫ��ʼ�����������
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            //��ʱ
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
