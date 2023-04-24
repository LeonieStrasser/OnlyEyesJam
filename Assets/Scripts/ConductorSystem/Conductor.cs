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
                RemoveConductorFromPartnerList(_otherConductor); // Hau  weg!
                myConductorManager.ReportDisconnectFromTo(this, _otherConductor);
            }
        }
    }


    void AddConductorToList(Conductor _newConductor)
    {
        contactedConductors.Add(_newConductor);
    }

    public void RemoveConductorFromPartnerList(Conductor _conductorToRemove) // Wird auch aufgerufen wenn zwei Gruppen voneinander getrennt werden und der Manager versucht die Disconnectoren zu checken
    {
        if (contactedConductors.Contains(_conductorToRemove))
            contactedConductors.Remove(_conductorToRemove);
        else
            Debug.LogWarning(this.name + " hat versucht " + _conductorToRemove.name + " von seiner Kontaktliste zu removen obwohl er nicht in seiner Liste war!!! WTF!???");
    }

    bool IsConductorInMyList(Conductor _conductorToFind)
    {
        if (contactedConductors.Contains(_conductorToFind))
        {
            return true;
        }
        return false;
    }

    public List<Conductor> GetAllContactPartnerConnectionsFromConductor()
    {
        List<Conductor> _workList = new List<Conductor>();
        List<Conductor> _closedList = new List<Conductor>();

        // Add Conductor to check in die Worklist
        _workList.Add(this);

        // Beginn Loop
        while (_workList.Count > 0) // Wenn Worklist count nicht 0 ist 
        {
            // -- Nimm den ersten und merk ihn dir
            Conductor _conductorInFocus = _workList[0];

            // Add all seine Partner in die WOrklist wenn sie nicht schon drin sind oder in der closed list sind
            foreach (var item in _conductorInFocus.contactedConductors)
            {
                if (!_workList.Contains(item) && !_closedList.Contains(item))
                {
                    _workList.Add(item);
                }
            }

            // pack ihn aus der worklist und in die Closed List
            _workList.Remove(_conductorInFocus);
            _closedList.Add(_conductorInFocus);

            /// -- Loop wiederholen
        }


        return _closedList;
    }
}
