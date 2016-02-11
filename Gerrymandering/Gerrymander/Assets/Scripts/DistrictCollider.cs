using UnityEngine;
using System.Collections;

public class DistrictCollider : MonoBehaviour {

	public PolygonCollider2D area; 
	public GameObject[] points; 

	// Use this for initialization
	void Start () {
		area = this.gameObject.GetComponent<PolygonCollider2D> (); 
		Vector3[] passIn = new Vector3[points.Length]; 
		for (int i = 0; i < points.Length; i++)
			passIn [i] = points [i].transform.position; 
		SetCollider (passIn); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Takes in an array of gameobjects, 
	/// strips out the vector3s, and passes
	/// the result to SetCollider(Vector3[])
	/// </summary>
	/// <param name="points">Array of GameObjects.</param>
	void SetCollider(GameObject[] points)
	{
		Vector3[] vertices = new Vector3[points.Length]; 
		for (int i = 0; i < points.Length; i++) 
		{
			vertices[i] = points[i].transform.position; 
		}
		SetCollider (vertices); 
	}

	/// <summary>
	/// Sets the collider2D according to an array of vector3s
	/// </summary>
	/// <param name="vertices">The vector3 vertex locations of the district shape</param>
	void SetCollider(Vector3[] vertices)
	{
		Vector2[] temp = new Vector2[vertices.Length];
		Vector3 tempTemp; 
		for (int i = 0; i < vertices.Length; i++) {
			temp[i] = new Vector2(vertices[i].x, vertices[i].y); 
			tempTemp = new Vector3(temp[i].x, temp[i].y, 0); 
			tempTemp = this.transform.InverseTransformPoint(tempTemp); 
			temp[i] = new Vector2(tempTemp.x, tempTemp.y); 
		}
		area.points = temp; 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("Detected 2D");
		if (other.gameObject.CompareTag ("Unit")) 
		{

		}
	}

}
