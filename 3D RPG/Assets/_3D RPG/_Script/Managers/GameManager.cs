using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager> 
{
    public CharactersStats playerStats;
    private CinemachineFreeLook followCamera;

    //接口列表，观察者
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    //加载的时候保存Manager
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharactersStats player)
    {
        playerStats = player;

        //注册玩家后，将相机绑定到玩家
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = player.transform;
            followCamera.LookAt = player.transform;
        }
    }

    //添加观察者，只执行一次，防止重复添加
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //移除观察者
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //通知观察者,广播
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}

