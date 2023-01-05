using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharactersStats playerStats;

    public void RigisterPlayer(CharactersStats player)
    {
        playerStats = player;
    }
}
