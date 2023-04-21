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

    bool CheckConnectionGroupsForConductor(Conductor _conductorToCheck, out ConductorList _foundInList)
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
            if (_partnersList != _reporterList) // Wenn es nicht die selbe Liste ist, hau alle Conductoren der einen liste A in Liste B und lösche dann Liste A
            {
                foreach (var item in _reporterList.allCunductors)
                {
                    _partnersList.allCunductors.Add(item);
                }
                // Reporterliste löschen
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
        bool _foundPartnerInAnyGroup = CheckConnectionGroupsForConductor(_contactPartner, out _partnersList); // Schau nach ob der partner in einer gruppe bereits existiert
        bool _foundReporterInAnyGroup = CheckConnectionGroupsForConductor(_reporter, out _reporterList); // Schau nach ob der reporter bereits in eienr Gruppe existiert

        // EIgentlich sollte die Gruppe die selbe sein
        if (_partnersList != _reporterList)
            Debug.LogError("Hier ist ein Bug! Die beiden Listen von Disconnectreporter und Partner sind verschiedene!!! WTF Ich hoffe dieser Error wird nie getriggert!");

        // reporter! Hast du noch Partner?
        // Wenn nein - du wirst aus deiner Gruppe geworfen
        if (_reporter.contactedConductors.Count == 0)
        {
            _reporterList.allCunductors.Remove(_reporter);
            SetDisconnectedItemFeedback(_reporter);
        }

        // Wenn ja -- UFF Wir müssen rausfinden ob die Gruppe getrennt werden muss --- wie!?
        CheckIfGroupWasDisconnected(_reporter, _contactPartner, _reporterList);
    }


    void CheckIfGroupWasDisconnected(Conductor _disconnector1, Conductor _disconnector2, ConductorList _listToCheck)
    {
        List<Conductor> _connectedGroupOfDisconnector1 = _disconnector1.GetAllContactPartnerConnectionsFromConductor();
        List<Conductor> _connectedGroupOfDisconnector2 = _disconnector2.GetAllContactPartnerConnectionsFromConductor();
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
