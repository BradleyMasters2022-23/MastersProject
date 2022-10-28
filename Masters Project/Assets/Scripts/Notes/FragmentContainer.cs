using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentContainer : MonoBehaviour {
    [SerializeField] private Fragment fragment;
    [SerializeField] private Color color;

    /// <summary>
    /// ensures that upgrade is not null and calls SetUp
    /// </summary>
    private void Start() {
        if (fragment is null) {
          Destroy(this);
        }

        if(fragment != null) {
          SetUp(fragment);
        }
    }

    /// <summary>
    /// called exactly once, initializes container
    /// </summary>
    public void SetUp(Fragment obj) {
        fragment = obj;
        GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// called when player walks into the object. eventually change to a button?
    /// </summary>
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            // TODO: trigger upgrade select screen.
            // buttons on USS call PlayerUpgradeManager.AddUpgrade() for the linked upgrade
            PlayerNotesManager.instance.FindFragment(fragment);
            Destroy(this.gameObject);
        }
    }
}
