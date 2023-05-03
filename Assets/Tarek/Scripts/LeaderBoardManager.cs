using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEditor.PackageManager;

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
    Coroutine addingNewPlayers;
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
        Login((res1) => UpdatePlayerName(PlayerName , (res2) => SendLbScore(50, (res3)=>   GetLeaderBoard())));
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
}

