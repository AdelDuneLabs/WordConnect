using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
//using UnityEditor.PackageManager;
using UnityEngine.UI;
using Unity.VisualScripting;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using TMPro;
using WordConnect;

public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager Instance;
    const string StatisticNameStr = "LB";
    [SerializeField] int NumberOfRows = 20;
    const string PlayerNameKeyStr = "PLAYERNAME_STR";
    string PlayerName
    {
        get => PlayerPrefs.GetString(PlayerNameKeyStr, "");
        set => PlayerPrefs.SetString(PlayerNameKeyStr, value);
    }
    [SerializeField] Button showLeader_Button;
    [SerializeField] GameObject parent, changeName_UI;
    [SerializeField] LeaderboardItem leaderboardItem;
    [SerializeField] Button changeName_Button;
    [SerializeField] TMP_InputField playerName_Input;
    List<LeaderboardItem> leaderboardItemsList = new List<LeaderboardItem>();
    Coroutine addingNewPlayers;
    Coroutine LB_routine;
    int numberOfElements = 0;
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
        changeName_UI.SetActive(false);
        parent.SetActive(false);
        showLeader_Button.onClick.AddListener(() =>
        {
            ShowLeaderboard();
        });
        showLeader_Button.interactable = false;
        RefreshLeaderboardData(() => showLeader_Button.interactable = true);
        leaderboardItem.gameObject.SetActive(false);
    }
    private void Awake()
    {
        Instance = this;
    }
    private void RefreshLeaderboardData(Action onFinish = null)
    {
        isInit = false;
        leadboardData = null;
        if (PlayerName == "")
        {
            ChangeName(() => Siginin(onFinish));
        }
        else
        {
        
        Siginin(onFinish);
        }
    }

    private void ChangeName(Action onFinish)
    {
        playerName_Input.text = "";
        changeName_Button.interactable = false;
        changeName_UI.SetActive(true);
        changeName_Button.onClick.AddListener(() => {
            onFinish?.Invoke();
            PlayerName = playerName_Input.text;
            changeName_Button.onClick.RemoveAllListeners();
        });
        onFinish += () => changeName_UI.SetActive(false);

    }
    public void OnChangeName(string name)
    {
        changeName_Button.interactable = name != "";
    }
    private void Siginin(Action onFinish)
    {
        Login((res1) => UpdatePlayerName(PlayerName, (res2) => SendLbScore(GameController.Instance.GamePoints, (res3) => GetLeaderBoard((res3) =>
        {
            leadboardData = res3;
            isInit = true;
            onFinish?.Invoke();
        }))));
    }
    public void UpdateScore(int newScore)
    {
        SendLbScore(GameController.Instance.GamePoints);
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
        numberOfElements = obj.Leaderboard.Count;
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
                for (int i = 0; i < Math.Min(NumberOfRows, numberOfElements); i++)
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

