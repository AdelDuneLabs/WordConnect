using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PopupWin : MonoBehaviour
{


    public static PopupWin instance { private set; get; }
    [SerializeField] private GameObject background;
    [SerializeField] private Text popupText;
    [SerializeField] private List<string> stringList;




    private void Awake()
    {
        instance = this;

    }


    public IEnumerator Show(bool addWord=false)
    {
        background.transform.localScale = Vector2.zero;
        background.gameObject.SetActive(true);
        popupText.text = "";
        var text = addWord ? stringList.LastOrDefault() :  stringList[Random.Range(0, stringList.Count-1)];
        background.transform.DOScale(1, 0.5f);

        StringBuilder sb = new StringBuilder();

        foreach (var item in text)
        {

            sb.Append(item);

            yield return new WaitForSeconds(0.01f);
            popupText.text = sb.ToString();


        }

        yield return new WaitForSeconds(1f);

        background.gameObject.SetActive(false);

    }



}
