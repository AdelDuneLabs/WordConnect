using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] TMP_Text rank_Text, name_Text, score_Text;
    [SerializeField] Color firstPlace_Color, otherPlaces_Color;
    [SerializeField] Image first_Image, scnd_Image, thrd_Image;
    [SerializeField] Image BG;
    public void Set(int rank, string name, int score)
    {
        BG.color = rank == 1 ? firstPlace_Color : otherPlaces_Color;
        first_Image.gameObject.SetActive(false);
        scnd_Image.gameObject.SetActive(false);
        thrd_Image.gameObject.SetActive(false);
        string rankText;
        switch (rank)
        {
            case 1:
                rankText = "1st";
                first_Image.gameObject.SetActive(true);
                break;
            case 2:
                rankText = "2nd";
                scnd_Image.gameObject.SetActive(true);
                break;
            case 3:
                rankText = "3rd";
                thrd_Image.gameObject.SetActive(true);
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
