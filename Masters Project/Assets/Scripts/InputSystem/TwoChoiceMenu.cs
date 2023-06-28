using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TwoChoiceMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mainPrompt;

    [SerializeField] UnityAction optionA;
    [SerializeField] UnityAction optionB;

    [SerializeField] TextMeshProUGUI optionAText;
    [SerializeField] TextMeshProUGUI optionBText;

    public void SetMainPrompt(string prompt)
    {
        mainPrompt.text= prompt;
    }

    public void Open(
        UnityAction actionA, string optionAPrompt, 
        UnityAction actionB, string optionBPrompt)
    {
        optionAText.text = optionAPrompt;
        optionA = actionA;

        optionBText.text = optionBPrompt;
        optionB= actionB;

        gameObject.SetActive(true);
    }

    public void SelectOptionA()
    {
        optionA.Invoke();
    }

    public void SelectOptionB()
    {
        optionB.Invoke();
    }
}
