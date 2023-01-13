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

    //战斗相关

    //承受伤害
    public void TakeDamage(CharactersStats attacker, CharactersStats defener)
    {
        //确保伤害不会小于0
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);

        //承受伤害，且血量不小于0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //暴击播放受击动画
        if (attacker.isCritical)
        {
            defener.GetComponentInChildren<Animator>().SetTrigger("Hit");
        }

        //更新血条
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //获得经验
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    //重载
    public void TakeDamage(int damage, CharactersStats defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);

        //更新血条
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    //获得当前状态下的随机伤害值
    private int CurrentDamage()
    {
        //从最小伤害和最大伤害间取随机
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        //判断暴击
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }
}
