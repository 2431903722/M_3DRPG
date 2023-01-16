using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    GameObject player;
    NavMeshAgent playerAgent;

    //是否在过渡中
    bool fadeFinished;

    //加载的时候保存Manager
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //注册观察者
        GameManager.Instance.AddObserver(this);

        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            //同场景传送
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            
            //不同场景传送
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //保存数据
        SaveManager.Instance.SavePlayerData();

        //不同场景的传送
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);

            //加载数据
            SaveManager.Instance.LoadPlayerData();

            //完成上述后中断协程
            yield break;
        }
        else
        {
            //同场景的传送
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();

            //传送前关闭agent
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //传送后重启agent
            playerAgent.enabled = true;

            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }

        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    //加载保存的场景
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //传送到第一个场景
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        //生成淡入淡出效果
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (sceneName != "")
        {
            //淡出
            yield return StartCoroutine(fade.FadeOut(2.5f));
            
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //保存数据
            SaveManager.Instance.SavePlayerData();

            //淡入
            yield return StartCoroutine(fade.FadeIn(2.5f));

            //结束协程
            yield break;
        }
    }

    //加载主菜单
    IEnumerator LoadMain()
    {
        //生成淡入淡出效果
        SceneFader fade = Instantiate(sceneFaderPrefab);

        //淡出
        yield return StartCoroutine(fade.FadeOut(2.5f));

        yield return SceneManager.LoadSceneAsync("Main");

        //淡入
        yield return StartCoroutine(fade.FadeIn(2.5f));
        
        yield break;
    }

    public void EndNotify()
    {
        //避免重复播放
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}