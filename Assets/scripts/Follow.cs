using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, null, "NewBehaviourScript")]
public class Follow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] Target;
    public Vector3 Offset;
    public int Cam;
    public bool IsDie;
    void Start()
    {
        Cam = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Die();
        Lock();
        if (Target[Cam] != null)
        {
            transform.position = Target[Cam].position + Offset;
        }
    }

    void Die()
    {
        if (Target[Cam] == null)
        {
            IsDie = true;
        }

        if (IsDie)
        {
            transform.position = transform.localPosition;
            IsDie = false;
        }
    }

    void Lock()
    {
        if (Target[0] == null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && Cam > 0)
            {
                Cam--;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && Cam + 1 < Target.Length)
            {
                Cam++;
            }
        }
    }
}
