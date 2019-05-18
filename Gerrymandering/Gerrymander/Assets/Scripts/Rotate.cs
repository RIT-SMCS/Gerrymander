using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    float rotation = 0.0f;

		// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf)
        {
            rotation = -Time.deltaTime * 125 * Mathf.PI;

            transform.Rotate(new Vector3(0.0f, 0.0f, rotation));       
        }
	}

    
}
