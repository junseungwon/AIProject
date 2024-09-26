using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{   
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(30, 0, 0);        
    }    
}
