using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererID : MonoBehaviour
{
    [SerializeField] int renderID;
    public bool spriteRenderer;

    public int RenderID { get { return renderID; } }
}
