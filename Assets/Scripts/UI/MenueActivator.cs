using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class MenueActivator : MonoBehaviour
{
    [SerializeField] Animator anim;

    public bool menuIsActive;
    public int gazedObjects = 0;
    public int GazedObjects
    {
        private set
        {
            gazedObjects = value;
            if (gazedObjects == 0)
            {
                // Wenn kein Men� Objekt mehr anfokussiert uist, kann es deaktiviert werden
                DefovusUI();
                menuIsActive = false;
            }
            else if (gazedObjects > 0 && !menuIsActive)
            {
                FocusUI();
                menuIsActive = true;
            }
            else if (gazedObjects < 0)
            {
                Debug.LogWarning("HIER STIMMT WAS NICHTR IHR PAPPNASEN!!! Es k�nnen nicht weniger als 0 Objekte im Men�focus assigned sein!!!");
            }
        }

        get
        {
            return gazedObjects;
        }
    }

    void FocusUI()
    {
        anim.SetBool("visible", true);
    }


    void DefovusUI()
    {
        anim.SetBool("visible", false);

    }



    [Button]
    public void LoginGazedObject()
    {
        GazedObjects++;
    }
    [Button]
    public void LogoutGazedObject()
    {
        GazedObjects--;
    }

}