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

    [SerializeField] Animator animator;

    private List<Image> imageBuffer;
    bool front;

    public void OpenUI(List<CollectableFragment> fragments)
    {
        imageBuffer = new List<Image>();

        fragmentsToload = fragments;
        template.gameObject.SetActive(true);
        string tempBuffer = "";
        foreach (CollectableFragment fragment in fragmentsToload)
        {
            Image cont = Instantiate(template, imageContainer).GetComponent<Image>();
            cont.gameObject.transform.localPosition= Vector3.zero;
            cont.sprite = fragment.Sprite();
            imageBuffer.Add(cont);
            tempBuffer += fragment.Text();
        }
        descText.text = tempBuffer;
        template.gameObject.SetActive(false);

        front = true;
        animator.SetBool("Front", front);

        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
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

        front = true;
        animator.SetBool("Front", front);

        template.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        

        imageContainer.transform.localScale = new Vector3(1, 1, 1);
        descText.text = fragment.Text();

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
        animator.SetBool("Front", true);
        front = true;
        descText.text = "";
    }

    /// <summary>
    /// Call animator to flip. Called via UI
    /// </summary>
    public void Flip()
    {
        front = !front;
        animator.SetBool("Front", front);
    }

    /// <summary>
    /// Swap data between normal and alt. Called via animator
    /// </summary>
    public void SwapData()
    {
        string newStr = "";

        for (int i = 0; i < imageBuffer.Count; i++)
        {
            if (front)
            {
                imageBuffer[i].sprite = fragmentsToload[i].Sprite();
                newStr += fragmentsToload[i].Text();
            }
            else
            {
                imageBuffer[i].sprite = fragmentsToload[i].AltSprite();
                newStr += fragmentsToload[i].AltText();
            }
        }

        // apply new text
        descText.text = newStr;
    }
}
