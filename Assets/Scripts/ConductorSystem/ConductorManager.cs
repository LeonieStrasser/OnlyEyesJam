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

    bool CheckConnectionGroupsForConductor(Conductor _conductorToCheck)
    {
        foreach (var item in allConnectedGroups)
        {
            if(item.FindConductorInList(_conductorToCheck))
            {
                return true;
            }
            
        }
        return false;
    }

    ConductorList AddNewConnectionGroup()
    {
        ConductorList _newConductorList = new ConductorList();
        allConnectedGroups.Add(_newConductorList);
        return _newConductorList;
    }

    public void ReportContactFromTo(Conductor _reporter, Conductor _contactPartner)
    {
        bool _foundPartnerInAnyGroup = CheckConnectionGroupsForConductor(_contactPartner); // Schau nach ob der partner in einer gruppe bereits existiert
        bool _foundReporterInAnyGroup = CheckConnectionGroupsForConductor(_reporter); // Schau nach ob der reporter bereits in eienr Gruppe existiert

        if(_foundPartnerInAnyGroup && !_foundReporterInAnyGroup) // Wenn Partner in ewiner Gruppe ist und reporter noch nicht, hau den Reporter einfach dazu
        {

        }
        else if(!_foundPartnerInAnyGroup && !_foundReporterInAnyGroup) // Wenn keiner von beiden in einer Gruppe existiert, erstell eine neue Gruppe und packe den reporter da rein.
        {
            ConductorList _newList = AddNewConnectionGroup();
        }
        else if(_foundPartnerInAnyGroup && _foundReporterInAnyGroup) // Wenn beide bereits in einer Gruppe existieren, check ob sie in der selben gruppe existieren --- uff case!!!
        {

        }
    }


}
