using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_Vfx : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);
        //Destroy(this.gameObject, GetComponent<VisualEffect>().main.duration);

    }

}
