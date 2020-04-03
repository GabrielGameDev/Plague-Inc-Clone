using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Continent : MonoBehaviour
{
    public string nome;
    public int totalPopulation;
    public float infecteds;
    public float deaths;

    public Animator animator;

    public bool infected;

    public Image infectedFill, deathFill;

}
