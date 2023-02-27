using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MapInitialize 
{
    bool initialized { get; }

    IEnumerator InitializeComponent();
}
