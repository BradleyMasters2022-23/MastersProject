using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePosition : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] float transitionTime;

    [SerializeField] Transform originalPos;
    [SerializeField] Transform targetPos;

    public void TriggerMoveEvent()
    {
        StartCoroutine(MoveToPos());
    }

    private IEnumerator MoveToPos()
    {
        ScaledTimer t = new ScaledTimer(delay);

        yield return new WaitUntil(t.TimerDone);

        t.ResetTimer(transitionTime);
        while(!t.TimerDone())
        {
            transform.position = Vector3.Lerp(originalPos.position, targetPos.position, t.TimerProgress());
            yield return null;
        }
        transform.position = targetPos.position;
    }
}
