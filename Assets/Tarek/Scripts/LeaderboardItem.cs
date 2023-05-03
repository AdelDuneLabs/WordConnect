using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] TMP_Text rank_Text, name_Text, score_Text;
    public void Set(int rank, string name, int score)
    {
        string rankText;
        switch (rank)
        {
            case 1:
                rankText = "1st";
                break;
            case 2:
                rankText = "2nd";
                break;
            case 3:
                rankText = "3rd";
                break;
            default:
                rankText = rank + "th";
                break;
        }
        rank_Text.text = rankText;
        name_Text.text = name;
        score_Text.text = score.ToString();
    }
}
