using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public List<Conductor> contactedConductors;
    ConductorManager myConductorManager;

    bool grounded;
    
    // Start is called before the first frame update
    void Start()
    {
        contactedConductors = new List<Conductor>();
        myConductorManager = FindObjectOfType<ConductorManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Conductor _otherConductor;
        if (collision.transform.TryGetComponent<Conductor>(out _otherConductor))
        {
            //  Der andere ist auch ein Conductor
            if (!IsConductorInMyList(_otherConductor)) // Schau mal nach ob er noch nicht in meiner Liste ist
            {                                         // Wenn nein
                AddConductorToList(_otherConductor);    // Pack dazu
                myConductorManager.ReportContactFromTo(this, _otherConductor);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Conductor _otherConductor;
        if (collision.transform.TryGetComponent<Conductor>(out _otherConductor))
        {
            //  Der andere ist auch ein Conductor
            if (IsConductorInMyList(_otherConductor)) // Schau mal nach ob er  in meiner Liste ist
            {                                         // Wenn ja
                contactedConductors.Remove(_otherConductor); // Hau  weg!
            }
        }
    }


    void AddConductorToList(Conductor _newConductor)
    {
        contactedConductors.Add(_newConductor);
    }

    void RemoveConductorToList(Conductor _conductorToRemove)
    {
        contactedConductors.Remove(_conductorToRemove);
    }

    bool IsConductorInMyList(Conductor _conductorToFind)
    {
        if(contactedConductors.Contains(_conductorToFind))
        {
            return true;
        }
        return false;
    }
}
