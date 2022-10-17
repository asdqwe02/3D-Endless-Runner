using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointHandler : MonoBehaviour
{
    public LaserController laser;

    private void OnTriggerStay(Collider other) // idk about this
    {
        if (other.tag == "EnemyEntity" && laser != null && !laser.targetList.Contains(other.transform))
        {
            laser.targetList.Add(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnemyEntity" && laser != null && laser.targetList.Contains(other.transform))
        {
            laser.targetList.Remove(other.transform);
        }
    }
    private void OnDisable()
    {
        if (laser != null)
        {
            laser.targetList.Clear();
            laser = null;
        }
    }
}
