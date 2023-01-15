using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;

    //加载的时候保存Manager
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
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
        if (sceneName != "")
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
        }

        //保存数据
        SaveManager.Instance.SavePlayerData();

        //结束协程
        yield break;
    }

    //加载主菜单
    IEnumerator LoadMain()
    {
        yield return SceneManager.LoadSceneAsync("Main");
        yield break;
    }
}