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

    //�Ƿ��ڹ�����
    bool fadeFinished;

    //���ص�ʱ�򱣴�Manager
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //ע��۲���
        GameManager.Instance.AddObserver(this);

        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            //ͬ��������
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            
            //��ͬ��������
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //��������
        SaveManager.Instance.SavePlayerData();

        //��ͬ�����Ĵ���
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);

            //��������
            SaveManager.Instance.LoadPlayerData();

            //����������ж�Э��
            yield break;
        }
        else
        {
            //ͬ�����Ĵ���
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();

            //����ǰ�ر�agent
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //���ͺ�����agent
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

    //���ر���ĳ���
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //���͵���һ������
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        //���ɵ��뵭��Ч��
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (sceneName != "")
        {
            //����
            yield return StartCoroutine(fade.FadeOut(2.5f));
            
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //��������
            SaveManager.Instance.SavePlayerData();

            //����
            yield return StartCoroutine(fade.FadeIn(2.5f));

            //����Э��
            yield break;
        }
    }

    //�������˵�
    IEnumerator LoadMain()
    {
        //���ɵ��뵭��Ч��
        SceneFader fade = Instantiate(sceneFaderPrefab);

        //����
        yield return StartCoroutine(fade.FadeOut(2.5f));

        yield return SceneManager.LoadSceneAsync("Main");

        //����
        yield return StartCoroutine(fade.FadeIn(2.5f));
        
        yield break;
    }

    public void EndNotify()
    {
        //�����ظ�����
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}