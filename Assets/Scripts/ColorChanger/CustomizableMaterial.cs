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

    private List<MeshRenderer> meshRendererInstancesInScene;
    public List<MeshRenderer> MeshRendererInstancesInScene
    {
        get
        {
            if (meshRendererInstancesInScene == null)
            {
                meshRendererInstancesInScene = new List<MeshRenderer>();
            }
            return meshRendererInstancesInScene;
        }

        set
        {
            if (meshRendererInstancesInScene == null)
            {
                meshRendererInstancesInScene = new List<MeshRenderer>();
            }
            meshRendererInstancesInScene = value;
        }
    }


}
