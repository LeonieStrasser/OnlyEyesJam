using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] ColorPaletteSetup[] allColorSetups;
    [SerializeField] ColorPaletteSetup currentSetup;


    [BoxGroup("Materialgroups to change Color")] [SerializeField] CustomizableMaterial stoneColorChangeMats;

    [Button]
    void LodeColorsInScene(ColorPaletteSetup _currentPalette)
    {
        // für jedes Material das CustomizableMaterial x hat to folgendes
        foreach (var mat in stoneColorChangeMats.materials)
        {
            //Check jeden Value von X´s Colorvalue Liste 
            foreach (var myColorVal in stoneColorChangeMats.colorValues)
            {
                string _nameToCompare = myColorVal.valueName;
                // Wenn die _currentPalette einen ColorValue mit dem selben string hat, setze hier im Colorvalue die Farbe von _currentPallettes entsprechendem ColorValue ein
                foreach (var set in _currentPalette.allColorValueSets)
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


}
