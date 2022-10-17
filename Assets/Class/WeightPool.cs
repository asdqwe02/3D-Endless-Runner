using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeightObject<T> 
{
    T obj;
    public int weight;
    public WeightObject(T obj, int weight)
    {
        this.obj = obj;
        this.weight = weight;
    }
    public T GetObject()
    {
        return obj;
    }
}

public class WeightPooler<T> // use this for rolling object with weight 
{
    int weightSum = 0;
    public List<WeightObject<T>> weightObjects;
    public void AddObjectToPool(T obj, int weight)
    {
        weightObjects.Add(new WeightObject<T>(obj, weight));
    }
    public WeightPooler(List<WeightObject<T>> weightObjects)
    {
        this.weightObjects = weightObjects;
    }
 
    public void CalculateTotalWeight()
    {
        foreach (var obj in weightObjects)
        {
            weightSum += obj.weight;
        }
    }
    public int GetWeightSum()
    {
        return weightSum;
    }
    public T RollPoolObjects() 
    {
        if (weightSum == 0)
            CalculateTotalWeight();
        int randomWeight = Random.Range(0, weightSum);
        int accumulatedWeight = 0;
        foreach (var weightObject in weightObjects)
        {
            accumulatedWeight += weightObject.weight;
            if (accumulatedWeight >= randomWeight)
            {
                return weightObject.GetObject();
            }
        }
        return default(T);
    }
    public void SortPool()
    {
        // sort the list by its object weight ascending
        weightObjects.Sort(delegate (WeightObject<T> obj1, WeightObject<T> obj2) { return obj1.weight.CompareTo(obj2.weight); });
    }

}

