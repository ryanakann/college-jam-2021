using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableUI : MonoBehaviour
{
    public string propertyName;

    private void Start()
    {
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }

    public virtual void Save()
    {

    }

    public virtual void Load()
    {

    }
}
