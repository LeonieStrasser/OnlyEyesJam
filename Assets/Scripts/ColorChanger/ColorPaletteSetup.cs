using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ColorSetup", menuName = "New Color Palette")]
[System.Serializable]
public class ColorPaletteSetup : ScriptableObject
{
    [BoxGroup("Background")] public ColorValueSetForOneMaterial[] allColorValueSets;
}
