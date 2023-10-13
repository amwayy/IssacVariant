using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointVisual : MonoBehaviour
{
    [SerializeField] private GameObject fillGameObject;

    public void Fill()
    {
        fillGameObject.SetActive(true);
    }

    public void Unfill()
    {
        fillGameObject.SetActive(false);
    }
}
