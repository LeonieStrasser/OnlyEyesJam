using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueActivator : GazeAwareBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] bool canActivate;
    [SerializeField] bool canDeactivate;

    private void Start()
    {


    }

    protected override void OnFocusStart()
    {
        base.OnFocusStart();

        if (canActivate)
        {
            FocusUI();

        }
    }

    protected override void OnFocusEnd()
    {
        base.OnFocusEnd();

        if (canDeactivate)
        {
            DefovusUI();

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
}
