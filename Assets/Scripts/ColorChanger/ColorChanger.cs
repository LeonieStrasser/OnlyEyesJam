using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] ColorPaletteSetup[] allColorSetups;
    [SerializeField] ColorPaletteSetup currentSetup;


    [BoxGroup("Materialgroups to change Color")] [SerializeField] CustomizableMaterial[] ColorChangeMatsOfType;

    [Button]
    void TestLodeColorsFromPalette()
    {
        // für jedes Material das CustomizableMaterial x hat to folgendes
        foreach (var mat in ColorChangeMatsOfType[0].materials)
        {
            //Check jeden Value von X´s Colorvalue Liste 
            foreach (var myColorVal in ColorChangeMatsOfType[0].colorValues)
            {
                string _nameToCompare = myColorVal.valueName;
                // Wenn die _currentPalette einen ColorValue mit dem selben string hat, setze hier im Colorvalue die Farbe von _currentPallettes entsprechendem ColorValue ein
                foreach (var set in currentSetup.allColorValueSets)
                {
                    // Geh jedes Set des scriptable Objects durch
                    foreach (var colorVal in set.allColorValues)
                    {
                        if (colorVal.valueName == _nameToCompare)
                        {
                            myColorVal.color = colorVal.color;
                        }

                    }
                }
            }
        }
    }


    [Button]
    public void SetRandomColorsInScene()
    {
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
            // für jedes Material das ColorValueSetForOneMaterial x hat do folgendes -- Denn jedes Material braucht das selebe Color Update
            foreach (var mat in ColorChangeMatsOfType[0].materials)
            {
                //Check jeden Value der Colorvalue Liste im ColorValueSetForOneMaterial
                foreach (var myColorVal in ColorChangeMatsOfType[0].colorValues)
                {
                    string _nameToCompare = myColorVal.valueName;
                    // Wenn die _currentPalette einen ColorValue mit dem selben string hat, setze hier im Colorvalue des materials die Farbe von _currentPallettes entsprechendem ColorValue ein
                    foreach (var set in _currentPalette.allColorValueSets)
                    {
                        // Geh jedes Set des scriptable Objects durch -- Die Sets existieren nur um die Farben einfacher einzustellen
                        foreach (var colorVal in set.allColorValues)
                        {
                            if (colorVal.valueName == _nameToCompare)
                            {
                                myColorVal.color = colorVal.color;
                                SetNewColor(mat, _nameToCompare, myColorVal.color);
                            }
                        }
                    }
                }
            }
        }
    }

    void SetNewColor(Material _material, string _changeColorName, Color _newColor)
    {
        _material.SetColor(_changeColorName, _newColor);
    }

}
