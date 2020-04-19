using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitForTap : MonoBehaviour
{
    Animator m_Animator;
    public GameObject TapToContinue;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = (Animator)GetComponent("Animator");
    }

    // Update is called once per frame
    void Update()
    {
        if (TapToContinue != null &&  Input.GetMouseButtonDown(0))
        {
            m_Animator.SetTrigger("ScreenTapped");
            Destroy(TapToContinue);
        }
    }

}
