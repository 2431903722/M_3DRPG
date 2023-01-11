using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> 
{
    public CharactersStats playerStats;

    //�ӿ��б��۲���
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    public void RigisterPlayer(CharactersStats player)
    {
        playerStats = player;
    }

    //��ӹ۲��ߣ�ִֻ��һ�Σ���ֹ�ظ����
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //�Ƴ��۲���
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //֪ͨ�۲���,�㲥
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}

