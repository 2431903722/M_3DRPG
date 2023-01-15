using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager> 
{
    public CharactersStats playerStats;
    private CinemachineFreeLook followCamera;

    //�ӿ��б��۲���
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    //���ص�ʱ�򱣴�Manager
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharactersStats player)
    {
        playerStats = player;

        //ע����Һ󣬽�����󶨵����
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = player.transform;
            followCamera.LookAt = player.transform;
        }
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

