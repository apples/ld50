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

    // Start is called before the first frame update
    void Start()
    {
        createClouds(0, 3.0f);
        createClouds(1, 2.5f);
        createClouds(2, 2.0f);
        createClouds(3, 1.5f);
        //createClouds(4, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if((int)player.transform.position.y > highestY){
            highestY++;
            
            var availableObjects = Physics.OverlapBox(
                new Vector3(0, (-50 + player.transform.position.y) / 2, 0),
                new Vector3(300, (50 + player.transform.position.y)/2, 300),
                Quaternion.identity,
                1<<7);
            int nextCloudSlot = 0;
            for(int i = 0; i < 10; i++){
                Vector3 coord = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range((highestY + 300) - 50, (highestY + 300) + 50), Random.Range(-300.0f, 300.0f));
                availableObjects[nextCloudSlot].transform.parent.parent.SetPositionAndRotation(coord, Quaternion.identity);
                nextCloudSlot++;


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
                        availableObjects[nextCloudSlot].transform.parent.parent.SetPositionAndRotation(coord, Quaternion.identity);
                        nextCloudSlot++;
                    }
                }
            }
        }
    }

    //every "level" is a 100 units high chunk, input refers to midpoint
    void createClouds(int level, float density)
    {
        for(int i=0; i < 1000 * density; i++){
            Vector3 coord = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range((100 * level) - 50, (100 * level) + 50), Random.Range(-300.0f, 300.0f));
            Instantiate(cloudPrefab, coord, Quaternion.identity);

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
                    Instantiate(cloudPrefab, coord, Quaternion.identity);
                }
            }
        }
    }
}
