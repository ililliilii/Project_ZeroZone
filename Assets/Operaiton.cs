using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operaiton : MonoBehaviour
{
    public bool GFW, GBC, GLT, GRT;
    public GameObject[] Gob;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GFW = Gob[0].GetComponent<Option>().FW;
        GBC = Gob[1].GetComponent<Option>().BC;
        GLT = Gob[2].GetComponent<Option>().LT;
        GRT = Gob[3].GetComponent<Option>().RT;
        if(GFW)
        {
            num = 0;
        }
        if (GBC)
        {
            num = 1;
        }
        if (GLT)
        {
            num = 2;

        }
        if (GRT)
        {
            num = 3;
        }
    }
}
