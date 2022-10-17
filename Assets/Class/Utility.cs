using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static WeightPooler<Expression> expressionWeightPool = new WeightPooler<Expression>(
       new List<WeightObject<Expression>>()
       {
            new WeightObject<Expression>(new PlusExpression(), weight:50),
            new WeightObject<Expression>(new SubtractExpression(), weight:50),
            new WeightObject<Expression>(new MultiplyExpression(), weight:40),
            new WeightObject<Expression>(new DivideExpression(), weight:40),
            new WeightObject<Expression>(new EqualExpression(), weight:25),
            new WeightObject<Expression>(new SquareRootExpression(), weight:25),
       }
    );
    public static List<T> GetRandomItemsFromList<T>(List<T> list, int number)
    {
        // this is the list we're going to remove picked items from
        List<T> tmpList = new List<T>(list);
        // this is the list we're going to move items to
        List<T> newList = new List<T>();

        // make sure tmpList isn't already empty
        while (newList.Count < number && tmpList.Count > 0)
        {
            int index = Random.Range(0, tmpList.Count);
            newList.Add(tmpList[index]);
            tmpList.RemoveAt(index);
        }
        return newList;
    }

    public static List<T> ConvertGameObjsToSCript<T>(List<GameObject> list) // convert list game object to list of the desire script
    {
        List<T> tmpList = new List<T>();
        foreach (var obj in list)
        {
            tmpList.Add(obj.GetComponent<T>());
        }
        return tmpList;
    }
    public static List<T> GetChildGameObjectWithScript<T>(Transform parent)
    {
        List<T> tmpList = new List<T>();
        foreach (Transform child in parent)
        {
            T temp = child.GetComponent<T>();
            if (temp != null)
            {
                tmpList.Add(temp);
            }
        }
        return tmpList;
    }

    public static int[] PartitionPowerLevel(int powerLevel, int n)
    {
        if (n <= 0)
            return new int[1] { 1 }; // hacky patchy fix for dumb and weird bug
        int k = powerLevel / n;
        int ct = powerLevel % n;
        int i;
        int[] arr = new int[n];
        for (i = 0; i < ct; i++)
        {
            arr[i] = k + 1;
        }
        for (; i < n; i++)
            arr[i] = k;
        return arr;
    }

    public static List<EntitySpawnPosition> GetSpawnPositionWithEntity(List<EntitySpawnPosition> list)
    {
        List<EntitySpawnPosition> tmpList = new List<EntitySpawnPosition>();
        foreach (var pos in list)
        {
            if (pos.entity != null)
                tmpList.Add(pos);
        }
        return tmpList;
    }
    public static List<PlayerEntity> GetPlayerEntityInSpawnPosition(List<EntitySpawnPosition> list)
    {
        List<PlayerEntity> tmpList = new List<PlayerEntity>();
        foreach (var pos in list)
        {
            if (pos.entity != null)
                tmpList.Add(pos.entity.GetComponent<PlayerEntity>());
        }
        return tmpList;
    }

    public static GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }
    public static IEnumerator PlayerSkillCD(float time, System.Action CooldownAction) // should be able to be use generallly as a cooldown timer
    {
        yield return new WaitForSeconds(time);
        CooldownAction();
    }

    public static List<Transform> GetChildWithTag(Transform parent, string tag)
    {
        List<Transform> tempList = new List<Transform>();
        foreach (Transform child in parent)
        {
            if (child.tag == tag)
                tempList.Add(child);
        }
        return tempList;
    }

    public static float QuadraticEquation(float a, float b, float c, int sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }
    public static Color ConvertColor(float red, float green, float blue, float alpha)
    {
        return new Color(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
    }
    public static Color ConvertColor(float red, float green, float blue)
    {
        return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
    }


}


