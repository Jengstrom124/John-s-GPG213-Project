using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Stomach : MonoBehaviour
{
    public FishContainer fishContainer;
    public IPredator iPredator;
    public GameObject dolphin;
    private Vector3 originalScale;

    public void Start()
    {
        originalScale = dolphin.transform.localScale;
    }

    public void OnTriggerEnter(Collider other)
    {
        IEdible edible = other.GetComponent<IEdible>();

        if (edible != null && other != other.isTrigger)
        {
            edible.GetEaten(iPredator);

            if (edible.GetInfo().edibleType == EdibleType.Food)
            {
                fishContainer.AddToStomach(other.GetComponent<FishBase>());
                dolphin.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.75f, 1, 0.2f);
                StartCoroutine(EatScaleDelayCoroutine());
                //print("in stomach count equals " +fishContainer.totalFoodAmount);
            }
        }
    }

    private IEnumerator EatScaleDelayCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        dolphin.transform.DOScale(originalScale, 0.5f);
    }
}
