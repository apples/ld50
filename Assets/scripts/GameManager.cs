using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cratePrefab;
    public GameObject balloonPrefab;
    public GameObject player;
    private int highestY = 100;
    private List<int> takenSlots = new List<int>();
    //private int iterateDistance;
    private int previousIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        createClouds(0, 3.0f);
        createClouds(1, 2.5f);
        createClouds(2, 2.0f);
        //createClouds(3, 1.5f);
        //createClouds(4, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if((int)player.transform.position.y > highestY){
            highestY++;
            
            var availableObjects = Physics.OverlapBox(
                new Vector3(0, (-50 + (player.transform.position.y - 25)) / 2, 0),
                new Vector3(200, (50 + (player.transform.position.y - 25))/2, 200),
                Quaternion.identity,
                1<<7);
            takenSlots = new List<int>();
            //iterateDistance = (availableObjects.Length / 5);
            previousIndex = 0;

            for(int i = 0; i < 10; i++){
                Vector3 coord = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range((highestY + 200) - 50, (highestY + 200) + 50), Random.Range(-300.0f, 300.0f));
                availableObjects[getRandomInList(availableObjects.Length)].transform.parent.parent.SetPositionAndRotation(coord, Quaternion.identity);


                float spawning = Random.value;
                if(spawning > .9f){
                    Instantiate(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity);
                }
                else if(spawning < .1f){
                    Instantiate(balloonPrefab, coord + new Vector3(0, 3, 0), Quaternion.identity);
                }

                while(Random.value > .3f){
                    float dir = Random.Range(-1.0f, 1.0f);
                    if(dir > 0){
                        if(dir > .5f){
                            coord += new Vector3(5, 0, 0);
                        }
                        else{
                            coord += new Vector3(-5, 0, 0);
                        }
                    }else{
                        if(dir < -.5f){
                            coord += new Vector3(0, 0, 5);
                        }
                        else{
                            coord += new Vector3(0, 0, -5);
                        }
                    }

                    dir = Random.Range(-1.0f, 1.0f);
                    if(dir > 0){
                        if(dir > .5f){
                            coord += new Vector3(0, 1, 0);
                        }
                        else{
                            coord += new Vector3(0, -1, 0);
                        }
                    }

                    if(!Physics.CheckSphere(coord, 1f)){
                        availableObjects[getRandomInList(availableObjects.Length)].transform.parent.parent.SetPositionAndRotation(coord, Quaternion.identity);
                    }
                }
            }
        }
    }

    //every "level" is a 100 units high chunk, input refers to midpoint
    void createClouds(int level, float density)
    {
        var numClouds = 0;
        for(int i=0; i < 500 * density; i++)
        {
            var radius = (Random.value + Random.value) / 2f * 295f + 5f;
            var angleRad = Random.Range(0f, Mathf.PI * 2f);

            var x = Mathf.Sin(angleRad) * radius;
            var z = Mathf.Cos(angleRad) * radius;

            Vector3 coord = new Vector3(x, Random.Range((100 * level) - 50, (100 * level) + 50), z);
            SpawnCloud(coord);

            while (Random.value < .75f)
            {
                var dir = Vector3.Scale(Random.onUnitSphere, new Vector3(2.5f, 1f, 2.5f));

                if (dir == Vector3.zero) continue;

                dir += dir.normalized * 2.5f;

                coord += dir;

                SpawnCloud(coord);
            }
        }

        void SpawnCloud(Vector3 coord)
        {
            ++numClouds;
            Instantiate(cloudPrefab, coord, Quaternion.identity);

            if (Random.value < .1f)
            {
                Instantiate(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity);
            }

            if (Random.value < .2f)
            {
                Instantiate(balloonPrefab, coord + new Vector3(0, 3, 0), Quaternion.identity);
            }
        }

        Debug.Log(numClouds);
    }

    private int getRandomInList(int length){
        // int num = previousIndex + iterateDistance;
        // if(takenSlots.Exists(x => x == num)){
        //     if(iterateDistance < 1){
        //         return Random.Range(0, 10);
        //     }
        //     iterateDistance--;
        //     num = getRandomInList();
        // }
        int num = Random.Range(0, length);
        while(takenSlots.Exists(x => x == num)){
            num = Random.Range(0, length);
        }
        takenSlots.Add(num);
        return num;
    }
}
