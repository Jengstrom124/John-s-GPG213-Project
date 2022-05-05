using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class Stomach : NetworkBehaviour
{
    public FishContainer fishContainer;
    public IPredator iPredator;
    public GameObject dolphin;
    private Vector3 originalScale;
    public bool canEat;

    public void Start()
    {
        originalScale = dolphin.transform.localScale;
        canEat = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        IEdible edible = other.GetComponent<IEdible>();
        IPredator predator = other.GetComponent<IPredator>();

        if (edible != null && other != other.isTrigger && IsServer)
        {
            edible.GetEaten(iPredator);

            if (edible.GetInfo().edibleType == EdibleType.Food && predator == null)
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
