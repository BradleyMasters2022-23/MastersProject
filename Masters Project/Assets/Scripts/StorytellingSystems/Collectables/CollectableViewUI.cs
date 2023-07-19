using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectableViewUI : MonoBehaviour
{
    [SerializeField] List<CollectableFragment> fragmentsToload;
    [SerializeField] RectTransform imageContainer;
    [SerializeField] RectTransform template;

    [SerializeField] TextMeshProUGUI descText;

    private List<Image> imageBuffer;
    bool front;

    public void OpenUI(List<CollectableFragment> fragments)
    {
        imageBuffer = new List<Image>();

        fragmentsToload = fragments;
        template.gameObject.SetActive(true);
        foreach (CollectableFragment fragment in fragmentsToload)
        {
            Image cont = Instantiate(template, imageContainer).GetComponent<Image>();
            cont.gameObject.transform.localPosition= Vector3.zero;
            cont.sprite = fragment.Sprite();
            imageBuffer.Add(cont);
            //descText.text += fragment.Text();
        }
        template.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        front = true;
        imageContainer.transform.localScale = new Vector3(1, 1, 1);
    }
    public void OpenUI(CollectableFragment fragment)
    {
        imageBuffer = new List<Image>();

        fragmentsToload = new List<CollectableFragment>{fragment};
        template.gameObject.SetActive(true);
        Image cont = Instantiate(template, imageContainer).GetComponent<Image>();
        cont.gameObject.transform.localPosition = Vector3.zero;
        cont.sprite = fragment.Sprite();
        imageBuffer.Add(cont);
        template.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        front = true;
        imageContainer.transform.localScale = new Vector3(1, 1, 1);
        //descText.text = fragment.Text();

    }

    public void CloseUI()
    {
        foreach(Image i in imageBuffer.ToArray())
        {
            if(i != null)
            {
                imageBuffer.Remove(i);
                Destroy(i.gameObject);
            }
        }
        descText.text = "";
    }

    public void Flip()
    {
        Debug.Log("Flip Called");
        front = !front;

        for(int i = 0; i < imageBuffer.Count; i++)
        {
            if(front)
            {
                imageBuffer[i].sprite = fragmentsToload[i].Sprite();
                imageContainer.transform.localScale = new Vector3(1, 1, 1);

                //descText.text += fragmentsToload[i].Text();
            }
            else
            {
                imageBuffer[i].sprite = fragmentsToload[i].AltSprite();
                imageContainer.transform.localScale = new Vector3(-1, 1, 1);

                //descText.text += fragmentsToload[i].AltText();
            }
        }
    }
}
