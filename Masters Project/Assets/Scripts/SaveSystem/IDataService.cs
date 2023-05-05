using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataService
{
    bool Save<T>(string dataPath, T Data);

    T Load<T>(string dataPath);

    void Delete(string dataPath);
}
