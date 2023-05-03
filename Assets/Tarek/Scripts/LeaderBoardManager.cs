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
    private void Start()
    {
        Login();
    }

    private void Login()
    {
        var req = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(req, OnLogin, OnError);

    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("****** PlayFab Error " + JsonUtility.ToJson(error));
    }

    private void OnLogin(LoginResult resault)
    {
        Debug.Log("****** PlayFab resault " + JsonUtility.ToJson(resault));
        SendLbScore(UnityEngine.Random.Range(1, 5));
        GetLeaderBoard();

    }

    public void SendLbScore(int score)
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
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnUpdatePlayerStatistic, OnError);
    }
    private void GetLeaderBoard()
    {
        GetLeaderboardRequest req = new GetLeaderboardRequest
        {
            StatisticName = StatisticNameStr,
            StartPosition = 0,
            MaxResultsCount = NumberOfRows
        };
        PlayFabClientAPI.GetLeaderboard(req, OnGetLeaderBoard, OnError);
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

