using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : MonoBehaviour
{
    public List<Reward> possibleRewards;
    public GameObject sparklePrefab;

    public GameObjectVariable itemContainer;

    public float rewardDelay = 1f;

    void Start()
    {
        Debug.Assert(possibleRewards != null);
        Debug.Assert(possibleRewards.Count > 0);
        Debug.Assert(sparklePrefab != null);
        Debug.Assert(itemContainer != null);
    }

    void Update()
    {
        rewardDelay -= Time.deltaTime;

        if (rewardDelay <= 0f)
        {
            Instantiate(sparklePrefab, transform.position, Quaternion.identity);

            var idx = Random.Range(0, possibleRewards.Count);
            var reward = possibleRewards[idx];

            var rot = reward.randomRotation ? Random.rotation : Quaternion.identity;

            Instantiate(reward.prefab, transform.position, rot, itemContainer.Value.transform);

            Destroy(this.gameObject);
        }
    }

    [System.Serializable]
    public class Reward
    {
        public GameObject prefab;
        public bool randomRotation;
    }
}
