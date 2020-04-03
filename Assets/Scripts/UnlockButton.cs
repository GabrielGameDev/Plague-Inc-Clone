using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockButton : MonoBehaviour
{
    public string skillName;
    public int cost;
    public string description;
    public GameObject unlockButton;

    public float transmission;
    public float death;
    public float infection;
    public float cure;

    public void SetSkillInfo()
    {
        LevelController.instance.SetSkillInfo(this);
    }
}
