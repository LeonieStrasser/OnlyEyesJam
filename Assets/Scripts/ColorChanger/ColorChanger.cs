using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] ColorPaletteSetup[] allColorSetups;
    [SerializeField] ColorPaletteSetup currentSetup;


    [BoxGroup("Materialgroups to change Color")] [SerializeField] CustomizableMaterial[] ColorChangeMatsOfType;








    public void SetRandomColorsInScene(out bool _warmColors)
    {
        ClearAllInstanceMaterialLists();
        SetAllMaterialInstances();
        SetRandomColorSet();
        LodeColorsFromPalette(currentSetup);

        _warmColors = currentSetup.warmColors;
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
            foreach (var renderInstance in matType.RendererInstancesInScene)
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
                                    //schau ob die renderinstance ein mesh oder spriterenderer ist
                                    if(!renderInstance.spriteRenderer)
                                    {
                                        //Wenn es ein Meshrenderer ist
                                        MeshRenderer _meshRenderer = renderInstance.transform.GetComponentInChildren<MeshRenderer>();
                                        if(_meshRenderer == null)
                                        {
                                            Debug.LogError("Hier ist ein RenderID script das nicht den richtigen rendererTyp eingestellt hat: " + renderInstance.gameObject.name);
                                        }

                                        myColorVal.color = colorVal.color;
                                        SetNewColor(_meshRenderer.material, _nameToCompare, myColorVal.color);
                                    }
                                    else
                                    {
                                        //wenn es ein Spriterenderer ist
                                        SpriteRenderer _spriteRenderer = renderInstance.transform.GetComponentInChildren<SpriteRenderer>();
                                        if (_spriteRenderer == null)
                                        {
                                            Debug.LogError("Hier ist ein RenderID script das nicht den richtigen rendererTyp eingestellt hat: " + renderInstance.gameObject.name);
                                        }

                                        myColorVal.color = colorVal.color;
                                        SetNewColor(_spriteRenderer.material, _nameToCompare, myColorVal.color);
                                    }

                                    
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
        RendererID[] allRenderers = FindObjectsOfType<RendererID>();
        foreach (var type in ColorChangeMatsOfType)
        {


            // Find all instances of the material in the scene
            foreach (var renderer in allRenderers)
            {
                if (renderer.RenderID == type.index)
                {
                    type.RendererInstancesInScene.Add(renderer);
                }
            }
        }



    }

    void ClearAllInstanceMaterialLists()
    {
        foreach (var type in ColorChangeMatsOfType)
        {
            type.RendererInstancesInScene.Clear();
        }
    }


}


