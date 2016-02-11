using UnityEngine;
using System.Collections;

public class DistrictCollider2 : MonoBehaviour {

	Mesh area; 
	public GameObject[] markers; 

	// Use this for initialization
	void Start () {
		markers = GameObject.FindGameObjectsWithTag ("Sphere"); 
		area = this.gameObject.GetComponent<MeshCollider> ().sharedMesh; 
		SetVertexList (markers); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetVertexList(GameObject[] points)
	{
		Vector3[] temp = new Vector3[2 * points.Length]; 
		for (int i = 0; i < points.Length; i++) {
			temp[i] = points[i].transform.position;
			temp[points.Length + i] = temp[i] + new Vector3(0,1,0); 
		}
		area.vertices = new Vector3[temp.Length];
		SetCollider (temp); 

	}

	void SetCollider(Vector3[] vertices)
	{
		area.vertices = vertices;
		//area.SetTriangles (vertices, 0); 
		this.gameObject.GetComponent<MeshCollider> ().sharedMesh = area; 
	}

	void OnTriggerStay(Collider other)
	{
		Debug.Log ("District2, colliding"); 
	}
}
