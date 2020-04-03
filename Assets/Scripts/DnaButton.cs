using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DnaButton : MonoBehaviour
{
    public void GetPoints()
    {
        LevelController.instance.GetDnaPoints();
        Destroy(gameObject);
    }
}
