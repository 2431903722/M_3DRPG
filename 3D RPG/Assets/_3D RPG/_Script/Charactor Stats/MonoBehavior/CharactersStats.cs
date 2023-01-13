using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    #region Read from CharacterData_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.maxHealth;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (characterData != null)
            {
                characterData.maxHealth = value;
            }
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentHealth;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (characterData != null)
            {
                characterData.currentHealth = value;
            }
        }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.baseDefence;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (characterData != null)
            {
                characterData.baseDefence = value;
            }
        }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentDefence;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (characterData != null)
            {
                characterData.currentDefence = value;
            }
        }
    }
    #endregion

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    //ս�����

    //�����˺�
    public void TakeDamage(CharactersStats attacker, CharactersStats defener)
    {
        //ȷ���˺�����С��0
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);

        //�����˺�����Ѫ����С��0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //���������ܻ�����
        if (attacker.isCritical)
        {
            defener.GetComponentInChildren<Animator>().SetTrigger("Hit");
        }

        //����Ѫ��
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //��þ���
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    //����
    public void TakeDamage(int damage, CharactersStats defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);

        //����Ѫ��
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    //��õ�ǰ״̬�µ�����˺�ֵ
    private int CurrentDamage()
    {
        //����С�˺�������˺���ȡ���
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        //�жϱ���
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("������" + coreDamage);
        }

        return (int)coreDamage;
    }
}
