using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersStats : MonoBehaviour
{
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

    //????????

    //????????
    public void TakeDamage(CharactersStats attacker, CharactersStats defener)
    {
        //????????????????0
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);

        //??????????????????????0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defener.GetComponentInChildren<Animator>().SetTrigger("Hit");
        }
    }

    //??????????????????????????
    private int CurrentDamage()
    {
        //????????????????????????????
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);

        //????????
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("??????" + coreDamage);
        }

        return (int)coreDamage;
    }
}
