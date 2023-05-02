using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] ColorPaletteSetup[] allColorSetups;
    [SerializeField] ColorPaletteSetup currentSetup;


    [BoxGroup("Materialgroups to change Color")] [SerializeField] CustomizableMaterial[] ColorChangeMatsOfType;


    private void Start()
    {

    }





    [Button]
    public void SetRandomColorsInScene()
    {
        ClearAllInstanceMaterialLists();
        SetAllMaterialInstances();
        SetRandomColorSet();
        LodeColorsFromPalette(currentSetup);
    }

    void SetRandomColorSet()
    {
        int _randomListIndex = Random.Range(0, allColorSetups.Length);
        currentSetup = allColorSetups[_randomListIndex];
    }

    void LodeColorsFromPalette(ColorPaletteSetup _currentPalette)
    {
        // Für jedes ColorValueSetForOneMaterial
        foreach (var matType in ColorChangeMatsOfType)
        {
            int typeIndex = matType.index;

            // für jedes Material das ColorValueSetForOneMaterial x hat do folgendes -- Denn jedes Material braucht das selebe Color Update
            foreach (var matInstance in matType.MeshRendererInstancesInScene)
            {
                //Check jeden Value der Colorvalue Liste im ColorValueSetForOneMaterial
                foreach (var myColorVal in matType.colorValues)
                {
                    string _nameToCompare = myColorVal.valueName;
                    // Wenn die _currentPalette einen ColorValue mit dem selben string hat, setze hier im Colorvalue des materials die Farbe von _currentPallettes entsprechendem ColorValue ein
                    foreach (var set in _currentPalette.allColorValueSets)
                    {
                        int setIndex = set.index;

                        if (typeIndex == setIndex)
                        {
                            // Geh das Set des scriptable Objects durch -- Die Sets existieren nur um die Farben einfacher einzustellen
                            foreach (var colorVal in set.allColorValues)
                            {
                                if ((colorVal.valueName == _nameToCompare))
                                {
                                    myColorVal.color = colorVal.color;
                                    SetNewColor(matInstance.sharedMaterial, _nameToCompare, myColorVal.color);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void SetNewColor(Material _material, string _changeColorName, Color _newColor)
    {
        _material.SetColor(Shader.PropertyToID(_changeColorName), _newColor);
    }

    void SetAllMaterialInstances()
    {
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
        foreach (var type in ColorChangeMatsOfType)
        {
            foreach (var mat in type.materials)
            {
                bool foundInstance = false;

                // Find all instances of the material in the scene
                foreach (var renderer in allRenderers)
                {
                    if (renderer.sharedMaterial == mat)
                    {
                        type.MeshRendererInstancesInScene.Add(renderer);
                        foundInstance = true;
                    }
                }

                if (!foundInstance)
                {
                    Debug.LogWarning("No instances of material " + mat.name + " were found in the scene.");
                }
            }
            Debug.Log("Material type " + type.materialType + " has found " + type.MeshRendererInstancesInScene.Count + " instance materials in the scene!");
        }
    }

    void ClearAllInstanceMaterialLists()
    {
        foreach (var type in ColorChangeMatsOfType)
        {
            type.MeshRendererInstancesInScene.Clear();
        }
    }

    [Button]
    void DEBUG()
    {
        Debug.Log("Steinmaterialzahl ist " + ColorChangeMatsOfType[0].MeshRendererInstancesInScene.Count);
    }
}


