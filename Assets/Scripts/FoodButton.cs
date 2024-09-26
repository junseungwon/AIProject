using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodButton : MonoBehaviour
{
    public GameObject food;

    MeshRenderer meshR;
    public Material grayMat;
    public Material greenMat;

    public FoodAgent fAgent;

    void Start()
    {        
        meshR = GetComponent<MeshRenderer>();        
    }

    private void Update()
    {
        if (fAgent.flag == true)
        {
            meshR.material = greenMat;
            transform.localPosition = new Vector3(0, 1f, 0);
        }
        else
        {
            meshR.material = grayMat;
            transform.localPosition = new Vector3(0, 0.2f, 0);
        }
    }    
}
