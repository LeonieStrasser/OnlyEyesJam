using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizableMaterial
{
    public string materialType;
    public int index;
    public Material[] materials;
    public ColorValue[] colorValues;

    private List<RendererID> rendererInstancesInScene;
    public List<RendererID> RendererInstancesInScene
    {
        get
        {
            if (rendererInstancesInScene == null)
            {
                rendererInstancesInScene = new List<RendererID>();
            }
            return rendererInstancesInScene;
        }

        set
        {
            if (rendererInstancesInScene == null)
            {
                rendererInstancesInScene = new List<RendererID>();
            }
            rendererInstancesInScene = value;
        }
    }

    


}
