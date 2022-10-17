using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tier
{
    public int minPower;
    public int maxPower;
    public Color color;
}

[CreateAssetMenu(fileName = "Entity Tier List", menuName ="New Entity Tier List")]
public class EntityTier : ScriptableObject
{
    public List<Tier> tiers;
    public float sizeMultiplier;

}
