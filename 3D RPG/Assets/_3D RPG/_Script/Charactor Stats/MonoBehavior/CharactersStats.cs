using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersStats : MonoBehaviour
{
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

    //战斗相关

    //承受伤害
    public void TakeDamage(CharactersStats attacker, CharactersStats defener)
    {
        //确保伤害不会小于0
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);

        //承受伤害，且血量不小于0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
    }

    //获得当前状态下的随机伤害值
    private int CurrentDamage()
    {
        //从最小伤害和最大伤害间取随机
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);

        //判断暴击
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }
}
