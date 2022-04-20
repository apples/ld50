using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class FuelCratesDisplay : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private RectTransform fuelCratesRequiredRectTransform;
        [SerializeField] private Transform fullCrate;
        [SerializeField] private Transform emptyCrate;
        
        
        private int currentCrateGoal = 0;
        private int currentCratesCollected = 0;
        private List<Transform> instantiatedUICrates;

        private int FUEL_CRATE_IMAGE_WIDTH = 80;

        private void Awake()
        {
            fullCrate.gameObject.SetActive(false);
            emptyCrate.gameObject.SetActive(false);
            instantiatedUICrates = new List<Transform>();
        }

        void Update()
        {
            if (currentCrateGoal != scoreManager.crateGoal || currentCratesCollected != scoreManager.NumCrates)
            {
                DeleteInstantiatedUICrates();
                
                currentCrateGoal = scoreManager.crateGoal;
                currentCratesCollected = scoreManager.NumCrates;
                
                fuelCratesRequiredRectTransform.sizeDelta = new Vector2(FUEL_CRATE_IMAGE_WIDTH * currentCrateGoal, fuelCratesRequiredRectTransform.rect.height);
                int displayedCollected = 0;
                for (int i = 0; i < currentCrateGoal; i++)
                {
                    Transform newCrate;
                    if (displayedCollected < currentCratesCollected)
                    {
                        newCrate = Instantiate(fullCrate, fuelCratesRequiredRectTransform.transform);
                        displayedCollected++;
                    }
                    else
                    {
                        newCrate = Instantiate(emptyCrate, fuelCratesRequiredRectTransform.transform);
                    }
                    RectTransform newFullCrateRectTransform = newCrate.GetComponent<RectTransform>();
                    newFullCrateRectTransform.anchoredPosition = new Vector3(FUEL_CRATE_IMAGE_WIDTH * i, 0);
                    instantiatedUICrates.Add(newCrate);
                    newCrate.gameObject.SetActive(true);
                }
            }
        }
        
        void DeleteInstantiatedUICrates()
        {
            for (int i = instantiatedUICrates.Count - 1; i >= 0; i--)
            {
                Transform crate = instantiatedUICrates[i];
                instantiatedUICrates.RemoveAt(i);
                Destroy(crate.gameObject);
            }
        }
    }
}
