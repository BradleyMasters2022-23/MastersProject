/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 13th, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Interface for segments
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SegmentInterface
{
    MapSegmentSO segmentInfo { get; }

    GameObject root { get; }



    void SetSO(MapSegmentSO newSegment);

    void Sync(Door syncPoint);

    IEnumerator ActivateSegment();

    IEnumerator DeactivateSegment(bool instant);

    Door GetExit();
}
