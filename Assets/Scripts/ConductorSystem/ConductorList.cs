using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConductorList
{
    public List<Conductor> allCunductors;

    public void Initialize()
    {
        allCunductors = new List<Conductor>();
    }

    public bool FindConductorInList(Conductor _cunductorToFind)
    {
        return allCunductors.Contains(_cunductorToFind);
    }
}
