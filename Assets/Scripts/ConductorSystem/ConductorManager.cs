using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductorManager : MonoBehaviour
{
    List<ConductorList> allConnectedGroups;

    private void Start()
    {
        allConnectedGroups = new List<ConductorList>();
    }

    public void ClearConductorList()
    {
        allConnectedGroups.Clear();
    }

    public bool CheckConnectionGroupsForConductor(Conductor _conductorToCheck, out ConductorList _foundInList)
    {
        foreach (var item in allConnectedGroups)
        {
            if (item.FindConductorInList(_conductorToCheck))
            {
                _foundInList = item;
                return true;
            }

        }
        _foundInList = null;
        return false;
    }

    ConductorList AddNewConnectionGroup()
    {
        ConductorList _newConductorList = new ConductorList();
        _newConductorList.Initialize();
        allConnectedGroups.Add(_newConductorList);
        return _newConductorList;
    }

    void AddConductorToList(Conductor _conductorToAdd, ConductorList _list)
    {
        _list.allCunductors.Add(_conductorToAdd);
    }

    public void ReportContactFromTo(Conductor _reporter, Conductor _contactPartner)
    {
        ConductorList _partnersList;
        ConductorList _reporterList;
        bool _foundPartnerInAnyGroup = CheckConnectionGroupsForConductor(_contactPartner, out _partnersList); // Schau nach ob der partner in einer gruppe bereits existiert
        bool _foundReporterInAnyGroup = CheckConnectionGroupsForConductor(_reporter, out _reporterList); // Schau nach ob der reporter bereits in eienr Gruppe existiert

        if (_foundPartnerInAnyGroup && !_foundReporterInAnyGroup) // Wenn Partner in einer Gruppe ist und reporter noch nicht, hau den Reporter einfach dazu
        {
            _partnersList.allCunductors.Add(_reporter);
        }
        else if (!_foundPartnerInAnyGroup && !_foundReporterInAnyGroup) // Wenn keiner von beiden in einer Gruppe existiert, erstell eine neue Gruppe und packe den reporter da rein.
        {
            ConductorList _newList = AddNewConnectionGroup();
            AddConductorToList(_reporter, _newList);
        }
        else if (_foundPartnerInAnyGroup && _foundReporterInAnyGroup) // Wenn beide bereits in einer Gruppe existieren, check ob sie in der selben gruppe existieren --- uff case!!!
        {
            if (_partnersList != _reporterList) // Wenn es nicht die selbe Liste ist, hau alle Conductoren der einen liste A in Liste B und l�sche dann Liste A
            {
                foreach (var item in _reporterList.allCunductors)
                {
                    _partnersList.allCunductors.Add(item);
                }
                // Reporterliste l�schen
                allConnectedGroups.Remove(_reporterList);
            }
            // Wenn beide schon in der selben Liste sind ist alles schickie!
        }


        SetConnectionGroupFeedback();
    }

    public void ReportDisconnectFromTo(Conductor _reporter, Conductor _contactPartner)
    {
        ConductorList _partnersList;
        ConductorList _reporterList;
        bool _foundPartnerInAnyGroup = CheckConnectionGroupsForConductor(_contactPartner, out _partnersList);
        bool _foundReporterInAnyGroup = CheckConnectionGroupsForConductor(_reporter, out _reporterList);

        if (_partnersList != _reporterList)
            Debug.LogError("Hier ist ein Bug! Die beiden Listen von Disconnectreporter und Partner sind verschiedene!!! WTF Ich hoffe dieser Error wird nie getriggert!");

        if (_reporter.contactedConductors.Count == 0)
        {
            _reporterList.allCunductors.Remove(_reporter);
            SetDisconnectedItemFeedback(_reporter);
            if (_reporterList.allCunductors.Count == 0) // check if list is empty
            {
                allConnectedGroups.Remove(_reporterList); // remove empty list
            }
        }
        else
        {
            List<Conductor> _connectedListOfReporter = new List<Conductor>();
            List<Conductor> _connectedListOfPartner = new List<Conductor>();
            bool _groupeSeperationNeeded = CheckIfGroupWasDisconnected(_reporter, _contactPartner, out _connectedListOfReporter, out _connectedListOfPartner);

            if (_groupeSeperationNeeded)
            {
                RemoveAListOfNamesFromList(_reporterList.allCunductors, _connectedListOfReporter);
                ConductorList _newConnectionGroup = AddNewConnectionGroup();

                foreach (var item in _connectedListOfReporter)
                {
                    _newConnectionGroup.allCunductors.Add(item);
                }

                if (_reporterList.allCunductors.Count == 0) // check if list is empty
                {
                    allConnectedGroups.Remove(_reporterList); // remove empty list
                }
            }

            SetConnectionGroupFeedback();
        }
    }


    bool CheckIfGroupWasDisconnected(Conductor _disconnectorReporter, Conductor _disconnectorPartner, out List<Conductor> _connectedGroupOfReporter, out List<Conductor> _connectedGroupOfPartner)
    {

        // Erstmal sicherstellen dass beide partner disconnected sind
        _disconnectorPartner.RemoveConductorFromPartnerList(_disconnectorReporter);

        // DAnn Listen mit connectorVerbindungen erstellen
        _connectedGroupOfReporter = _disconnectorReporter.GetAllContactPartnerConnectionsFromConductor();
        _connectedGroupOfPartner = _disconnectorPartner.GetAllContactPartnerConnectionsFromConductor(); // Der Contact Partner hat m�glicherweise noch nicht selbst disconnected deshal�b ist dioese Liste mit Vorsicht zu genie�en ^^

        // Dann checken ob listen gleiche items drin haben
        if (CompareTwoLists(_connectedGroupOfReporter, _connectedGroupOfPartner)) // Wenn beide Listen gleiche Namen enthalten
        {
            return false; // __ Weil die Gruppe nicht disconnected wurde
        }
        return true; // Wenn es hier ankommt wurde die Gruppe disconnected
    }

    bool CompareTwoLists(List<Conductor> _List1, List<Conductor> _List2)
    {
        foreach (var item1 in _List1)
        {
            foreach (var item2 in _List2)
            {
                if (item1 == item2)
                {
                    // In beiden Gruppen taucht der selbe name auf! Hei�t es ist dieselbe gruppe
                    return true;
                }
            }
        }
        return false;
    }

    void RemoveAListOfNamesFromList(List<Conductor> _SourceList, List<Conductor> _ListToRemove)
    {
        foreach (var item in _ListToRemove)
        {
            _SourceList.Remove(item);
        }
    }

    void SetConnectionGroupFeedback()
    {
        foreach (var item in allConnectedGroups)
        {
            foreach (var conductor in item.allCunductors)
            {
                conductor.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    void SetDisconnectedItemFeedback(Conductor _disconnectedConductor)
    {
        _disconnectedConductor.GetComponentInChildren<MeshRenderer>().material.color = Color.gray;
    }




}
