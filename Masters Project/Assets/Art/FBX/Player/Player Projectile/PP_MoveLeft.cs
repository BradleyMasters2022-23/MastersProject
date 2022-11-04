using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_MoveLeft : MonoBehaviour
{
    Vector3 left;
    public float ppSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            left = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + ppSpeed);
            gameObject.transform.position = left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            left = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - ppSpeed);
            gameObject.transform.position = left;
        }
    }
}
