using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class DistortionWallAmbientBuilder : MonoBehaviour
{
    [System.Serializable]
    private enum Axis
    {
        X, Y, Z
    }

    [SerializeField] Object ambianceEmitter;

    [SerializeField] int accuracy = 3;
    [SerializeField] Axis faceDirection;

    [Button]
    public void BuildWall()
    {
        float dist = 0;

        // Get the width. Easiest way is to temporarily add a box collider
        BoxCollider temp = gameObject.AddComponent<BoxCollider>();
        switch(faceDirection)
        {
            case Axis.X:
                {
                    dist = temp.size.x;
                    break;
                }
            case Axis.Y:
                {
                    dist = temp.size.y;
                    break;
                }
            case Axis.Z:
                {
                    dist = temp.size.z;
                    break;
                }
        }
        DestroyImmediate(temp);
        Debug.Log(dist);

        // get dist between each
        float objSpacing = dist/accuracy;
        GameObject[] spawned = new GameObject[accuracy];

        // spawn in each node
        for(int i = 0; i < spawned.Length; i++)
        {
            spawned[i] = PrefabUtility.InstantiatePrefab(ambianceEmitter, transform) as GameObject;
            Vector3 newPos = Vector3.zero;
            newPos.z = (i * objSpacing);
            newPos.z -= (dist / 2) - objSpacing/2;
            spawned[i].transform.localPosition= newPos;
        }
    }

    private void Awake()
    {
        Destroy(this);
    }
}
