using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEditor.PackageManager;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class LeaderBoardManager : MonoBehaviour
{
    const string StatisticNameStr = "LB";
    const int NumberOfRows = 10;
    const string PlayerNameKeyStr = "PLAYERNAME_STR";
    string PlayerName
    {
        get => PlayerPrefs.GetString(PlayerNameKeyStr, "Player");
        set => PlayerPrefs.SetString(PlayerNameKeyStr, value);
    }
    [SerializeField] Button showLeader_Button;
    [SerializeField] GameObject parent;
    [SerializeField] LeaderboardItem leaderboardItem;
    List<LeaderboardItem> leaderboardItemsList = new List<LeaderboardItem>();
    Coroutine addingNewPlayers;
    Coroutine LB_routine;
    bool isInit;
    GetLeaderboardResult leadboardData;
    [ContextMenu("Add random players")]
    void AddRandomPlayers()
    {
        if (addingNewPlayers!=null)
        {
            StopCoroutine(addingNewPlayers);
        }
        addingNewPlayers = StartCoroutine(Adding());
        IEnumerator Adding()
        {
            for (int i = 0; i < 10; i++)
            {

                var req = new LoginWithCustomIDRequest
                {
                    CustomId = UnityEngine.Random.Range(1000, 3000000000).ToString(),
                    CreateAccount = true
                };
                PlayFabClientAPI.LoginWithCustomID(req, (resL) =>
                {
                    GetLeaderBoard((res) =>
                    {

                        UpdatePlayerName(GenerateName(), (res) => SendLbScore(UnityEngine.Random.Range(5, 20)));

                    });


                }, OnError);
                yield return new WaitForSeconds(2);

            }
            Debug.Log("Finished ");
        }


        string GenerateName()
        {
            int len = UnityEngine.Random.Range(5, 8);
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[UnityEngine.Random.Range(0, consonants.Length)].ToUpper();
            Name += vowels[UnityEngine.Random.Range(0, vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[UnityEngine.Random.Range(0, consonants.Length)];
                b++;
                Name += vowels[UnityEngine.Random.Range(0, vowels.Length)];
                b++;
            }

            return Name;


        }
    }

    private void Start()
    {
        parent.SetActive(false);
        showLeader_Button.onClick.AddListener(() =>
        {
            ShowLeaderboard();
        });
        showLeader_Button.interactable = false;
        RefreshLeaderboardData(() => showLeader_Button.interactable = true);
    }

    private void RefreshLeaderboardData(Action onFinish = null)
    {
        isInit = false;
        leadboardData = null;
        Login((res1) => UpdatePlayerName(PlayerName, (res2) => SendLbScore(50, (res3) => GetLeaderBoard((res3) =>
        {
            leadboardData = res3;
            isInit = true;
            onFinish?.Invoke();
        }))));
    }

    private void Login(Action<LoginResult> onComplete = null)
    {
        var req = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        onComplete += OnLogin;
        PlayFabClientAPI.LoginWithCustomID(req, onComplete, OnError);
        void OnLogin(LoginResult resault)
        {
            Debug.Log("****** PlayFab resault " + JsonUtility.ToJson(resault));

        }
    }

    private void UpdatePlayerName(string playerName, Action<UpdateUserTitleDisplayNameResult> onComplete = null)
    {
        onComplete += OnNameChange;
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = playerName }, onComplete, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("****** PlayFab Error " + JsonUtility.ToJson(error));
    }



    private void OnNameChange(UpdateUserTitleDisplayNameResult obj)
    {
        Debug.Log("****** PlayFab NameChange " + JsonUtility.ToJson(obj));
    }

    public void SendLbScore(int score, Action<UpdatePlayerStatisticsResult> onComplete = null)
    {
        Debug.Log("***** LB add score " + score);
        UpdatePlayerStatisticsRequest req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>() {
         new StatisticUpdate {
             StatisticName  = StatisticNameStr
             , Value = score
         }
        }
        };
        onComplete += OnUpdatePlayerStatistic;
        PlayFabClientAPI.UpdatePlayerStatistics(req, onComplete, OnError);
    }
    private void GetLeaderBoard(Action<GetLeaderboardResult> onComplete = null)
    {
        GetLeaderboardRequest req = new GetLeaderboardRequest
        {
            StatisticName = StatisticNameStr,
            StartPosition = 0,
            MaxResultsCount = NumberOfRows
        };
        onComplete += OnGetLeaderBoard;
        PlayFabClientAPI.GetLeaderboard(req, onComplete, OnError);
    }

    private void OnGetLeaderBoard(GetLeaderboardResult obj)
    {
        Debug.Log("****** PlayFab GetLeaderBoard " + JsonUtility.ToJson(obj));
    }

    private void OnUpdatePlayerStatistic(UpdatePlayerStatisticsResult obj)
    {
        Debug.Log("****** PlayFab UpdatePlayerStatistic " + JsonUtility.ToJson(obj));
    }
    void ShowLeaderboard()
    {
        if (LB_routine!=null)
        {
            StopCoroutine(LB_routine);
        }
        LB_routine = StartCoroutine(ShowingLB());
        IEnumerator ShowingLB()
        {
            foreach (var item in gameObject.GetComponentsInChildren<LeaderboardItem>(true))
            {
                if (item != leaderboardItem)
                {
                    item.gameObject.SetActive(false);
                    Destroy(item.gameObject);
                }
            }
            leaderboardItemsList.Clear();
            RefreshLeaderboardData();
            showLeader_Button.gameObject.SetActive(false);
            if (leaderboardItemsList.Count <=0)
            {
                for (int i = 0; i < NumberOfRows; i++)
                {
                    var newItem = Instantiate(leaderboardItem, leaderboardItem.transform.parent, true);
                    newItem.transform.localScale = Vector3.one;
                    leaderboardItemsList.Add(newItem);
                }
            }
            yield return new WaitUntil(() => isInit = true && leadboardData !=null );

            for (int i = 0; i < leaderboardItemsList.Count; i++)
            {
                LeaderboardItem newItem = leaderboardItemsList[i];
                PlayerLeaderboardEntry playerLeaderboardEntry = leadboardData.Leaderboard[i];
                newItem.Set(playerLeaderboardEntry.Position+1, playerLeaderboardEntry.DisplayName, playerLeaderboardEntry.StatValue);
            }

            parent.SetActive(true);

        }
    }
    public void CloseLeaderboard()
    {
        if (LB_routine != null)
        {
            StopCoroutine(LB_routine);
        }
        LB_routine = StartCoroutine(HidingLB());
        IEnumerator HidingLB()
        {
            yield return null;
            parent.SetActive(false);
            showLeader_Button.gameObject.SetActive(true);
            showLeader_Button.interactable = true;
        }

    }
}

