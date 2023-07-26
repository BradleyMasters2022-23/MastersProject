/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 12th, 2022
 * Last Edited - April 12th, 2022 by Ben Schuster
 * Description - Controls enemy utility for healthbars
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyHealthbar : ResourceBarUI
{
    [Header("On Damaged")]

    [Tooltip("Whether to show healthbar no matter what on damaged")]
    [SerializeField] private bool enableShowOnDamaged;
    [Tooltip("How long to show healthbar for when damaged")]
    [SerializeField] private float onDamagedDuration;
    /// <summary>
    /// Whether the target was recently damaged
    /// </summary>
    private bool recentlyDamaged;
    /// <summary>
    /// Tracker for damaged duration
    /// </summary>
    private ScaledTimer timer;

    [Header("Distance Hiding")]

    [Tooltip("Whether to hide the UI outside of weapon range")]
    [SerializeField] private bool hideOutOfRange;
    [Tooltip("Additional distance offset for this target")]
    [SerializeField] private float distanceOffset;
    /// <summary>
    /// Reference to target to hide UI from
    /// </summary>
    private Transform target;
    /// <summary>
    /// Range to load within
    /// </summary>
    private float loadDistance = 30;
    /// <summary>
    /// Whether the target is within range
    /// </summary>
    private bool inRange;
    /// <summary>
    /// Reference to player gun
    /// </summary>
    private PlayerGunController gunRef;

    [SerializeField] protected bool preserveOffset = true;
    float verticalOffset;

    protected override void Awake()
    {
        base.Awake();
        verticalOffset = transform.localPosition.y;

        timer = new ScaledTimer(onDamagedDuration, false);
    }

    private void Start()
    {
        PlayerGunController rangeTemp = FindObjectOfType<PlayerGunController>();
        if (rangeTemp != null)
            loadDistance = rangeTemp.GetMaxRange();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        CheckDistance();
        CheckRecentlyDamaged();


        if(_targetData != null && _targetData.IsEmptied())
        {
            if (coreReference.gameObject.activeInHierarchy)
                coreReference.gameObject.SetActive(false);
            return;
        }

        if(hideOutOfRange)
        {
            if(enableShowOnDamaged)
            {
                coreReference.gameObject.SetActive(inRange || recentlyDamaged);
            }
            else
            {
                coreReference.gameObject.SetActive(inRange);
            }
        }

        // keep upright around its center
        if(preserveOffset && coreReference.gameObject.activeInHierarchy)
        {
            Vector3 pos = transform.parent.position + Vector3.up * verticalOffset;
            transform.position = pos;
        }
    }

    private void CheckDistance()
    {
        if (target == null || gunRef == null)
        {
            if (PlayerTarget.p != null)
            {
                target = PlayerTarget.p.Center;

                gunRef = target.GetComponentInParent<PlayerGunController>();

                if(gunRef != null)
                {
                    loadDistance = gunRef.GetMaxRange();
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            loadDistance = gunRef.GetMaxRange();

            float dist = Vector3.Distance(target.position, coreReference.position);

            if(dist <= loadDistance + distanceOffset)
            {
                inRange = true;
            }
            else if(dist > loadDistance + distanceOffset)
            {
                inRange = false;
            }
        }
    }

    private void CheckRecentlyDamaged()
    {
        recentlyDamaged = !timer.TimerDone();
    }

    public void Damaged()
    {
        timer.ResetTimer();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (enableShowOnDamaged && _targetResource != null)
        {
            _targetResource.onDamagedEvent += Damaged;
        }
    }

    private void OnDisable()
    {
        if (enableShowOnDamaged && _targetResource != null)
        {
            _targetResource.onDamagedEvent -= Damaged;
        }
    }
}
