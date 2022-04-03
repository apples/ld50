using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cratePrefab;
    public GameObject balloonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i < 10000; i++){
            Vector3 coord = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range(0, i/20), Random.Range(-300.0f, 300.0f));
            Instantiate(cloudPrefab, coord, Quaternion.identity);

            if(Random.value > .9f){
                Instantiate(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity);
            }

            if(Random.value > .95f){
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
