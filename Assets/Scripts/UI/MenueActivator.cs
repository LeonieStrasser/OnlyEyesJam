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
                // Wenn kein Menü Objekt mehr anfokussiert uist, kann es deaktiviert werden
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
                Debug.LogWarning("HIER STIMMT WAS NICHTR IHR PAPPNASEN!!! Es können nicht weniger als 0 Objekte im Menüfocus assigned sein!!!");
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

    public void RelodeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

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
