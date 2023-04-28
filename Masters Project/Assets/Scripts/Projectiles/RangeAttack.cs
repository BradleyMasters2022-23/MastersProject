/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Abstract base class for all ranged attacks
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangeAttack : Attack
{
    #region Variables

    [Header("===== Ranged Attack =====")]

    /// <summary>
    /// Whether or not this ranged attack is currently active
    /// </summary>
    protected bool active;
    /// <summary>
    /// Audio source for this object
    /// </summary>
    private AudioSource source;

    [Header("Ranged Setup")]

    [SerializeField] protected bool shotByPlayer;
    [SerializeField] protected LayerMask hitLayers;
    [SerializeField] protected LayerMask worldLayers;

    /// <summary>
    /// Owner of the ranged attacks
    /// </summary>
    protected GameObject owner;

    [Header("Ranged Gameplay")]

    [Tooltip("Base speed this attack moves at")]
    [SerializeField] protected float speed;

    [Tooltip("Range of this ranged attack")]
    [SerializeField] protected float range;

    [Tooltip("Maximum hits this projectile can do against a target")]
    [SerializeField] protected int maxHits;

    [Header("Ranged Effects")]

    [Tooltip("Sound when this bullet is shot")]
    [SerializeField] protected AudioClipSO onActivateSFX;

    [Tooltip("Sound effect that plays when hitting anything")]
    [SerializeField] protected AudioClipSO hitSFX;

    [Tooltip("Visual effect that plays when hitting anything")]
    [SerializeField] protected GameObject onHitVFX;

    #endregion

    /// <summary>
    /// Initialize any necessary variables needing to be passed in. 
    /// </summary>
    /// <param name="_damageMultiplier">multiplier for the damage. % based.</param>
    /// <param name="_speedMultiplier">multiplier for the speed. % based.</param>
    public virtual void Initialize(float _damageMultiplier, float _speedMultiplier, float maxRange, GameObject owner, bool _shotByPlayer = false)
    {
        active = false;

        source = GetComponent<AudioSource>();

        damage *= Mathf.FloorToInt(_damageMultiplier);
        speed *= _speedMultiplier;
        shotByPlayer = _shotByPlayer;
        range = maxRange;
        this.owner = owner;
        
        // Activate the projectile after being initialized
        Activate();
    }

    #region Abstract Functions

    protected abstract void UniqueActivate();

    /// <summary>
    /// Perform whatever flying behavior for this ranged attack
    /// </summary>
    protected abstract void Fly();

    /// <summary>
    /// Check if this ranged attack should end
    /// </summary>
    /// <returns>Whether the range attack life is over</returns>
    protected abstract bool CheckLife();

    /// <summary>
    /// Inturrupt this ranged attack
    /// </summary>
    public abstract void Inturrupt();

    #endregion

    #region Virtual Functions

    /// <summary>
    /// Activate the ranged attack
    /// </summary>
    public override void Activate()
    {
        if (hitTargets != null)
            hitTargets.Clear();
        else
            hitTargets = new List<Target>();

        onActivateSFX?.PlayClip(transform);

        UniqueActivate();

        active = true;
    }

    /// <summary>
    /// Activate any visual/audio effects when hitting a target
    /// </summary>
    /// <param name="impactPoint"></param>
    /// <param name="hitNormal"></param>
    protected override void Hit(Vector3 impactPoint, Vector3 hitNormal)
    {
        // Spawn in whatever its told to, if able
        if (onHitVFX != null)
        {
            GameObject t = Instantiate(onHitVFX, impactPoint, Quaternion.identity);
            t.transform.LookAt(t.transform.position + hitNormal);
        }

        hitSFX.PlayClip(transform);
    }

    /// <summary>
    /// Apply damage, check if max hits has been reached
    /// </summary>
    /// <param name="target"></param>
    protected virtual void ApplyDamage(Transform target, Vector3 damagePoint)
    {
        bool dealtDamage = DealDamage(target, damagePoint);

        // if damage was dealt and max targets reached, end projectile
        if (dealtDamage && hitTargets.Count >= maxHits)
            End();
    }

    /// <summary>
    /// What happens when this ranged attack ends
    /// TODO later - send back to pooler
    /// </summary>
    protected virtual void End()
    {
        if (spawnProjectileOnEnd && onEndPrefab != null)
            Instantiate(onEndPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    #endregion

    #region Getters & Setters

    /// <summary>
    /// Base speed this attack moveds at
    /// </summary>
    public float Speed
    {
        get { return speed; }
    }

    public float GetDamage()
    {
        return damage;
    }

    public void ChangeDamageTo(int newDamage)
    {
        damage = newDamage;
    }

    public bool GetShotByPlayer()
    {
        return shotByPlayer;
    }

    #endregion
}
