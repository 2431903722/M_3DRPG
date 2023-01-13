using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]

public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]

    public int maxHealth;

    public int currentHealth;

    public int baseDefence;

    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    //等级相关
    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;

    //让相关数值随等级增长
    public float LeveMultiplier
    {
        get{ return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LeveUp();
        }
    }

    private void LeveUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LeveMultiplier);

        maxHealth = (int)(maxHealth * LeveMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP!" + currentLevel + "Max Health: " + maxHealth);
    }
}
