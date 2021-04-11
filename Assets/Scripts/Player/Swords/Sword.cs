using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Create/Sword", order = int.MaxValue)]
public class Sword : ScriptableObject
{
    public uint ID = 0;
    public SwordLocationSprites swordLocationSprites;
}