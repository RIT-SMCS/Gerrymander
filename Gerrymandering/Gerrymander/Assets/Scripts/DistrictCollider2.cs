using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class DistrictCollider2 : MonoBehaviour {

	//public PolygonCollider2D area; 
	Mesh mesh;// = new Mesh(); 
	public GameObject[] points;
	
	public List<GameObject> units = new List<GameObject> (); 
	public int[] rgb;
	public Affiliation winner; 
	bool calcIt = false; 

	public DistrictCollider2(GameObject[] points)
	{
		SetCollider (points); 
	}
	
	// Use this for initialization
	void Awake () {
		mesh = new Mesh (); 
		this.GetComponent<MeshFilter> ().mesh = mesh; 
		rgb = new int[3];
		for (int i = 0; i< rgb.Length; i++)
			rgb [i] = 0; 
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
	public void SetCollider(GameObject[] points)
	{
		//Debug.Log ("SetCollider called"); 
		Vector3[] vertices = new Vector3[points.Length]; 
		for (int i = 0; i < points.Length; i++) 
		{
			vertices[i] = points[i].transform.position; 
		}
		//Debug.Log ("Mesh is" + mesh); 
		//Debug.Log (vertices);
		//Debug.Log (mesh.vertices); 
		mesh.vertices = vertices; 
		SetCollider (vertices); 
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
	}
	
	/// <summary>
	/// Sets the collider2D according to an array of vector3s
	/// </summary>
	/// <param name="vertices">The vector3 vertex locations of the district shape</param>
	public void SetCollider(Vector3[] vertices)
	{
		Vector2[] temp = new Vector2[vertices.Length];
		Vector3 tempTemp; 
		for (int i = 0; i < vertices.Length; i++) {
			//take just the relevant x and z coordinates...
			temp[i] = new Vector2(vertices[i].x, vertices[i].z); 
			//And now we have to transform them into local coordinates, for use in the collider
			tempTemp = new Vector3(temp[i].x, 0, temp[i].y); 
			tempTemp = this.transform.InverseTransformPoint(tempTemp); 
			temp[i] = new Vector2(tempTemp.x, tempTemp.z); 
		}
		//area.points = temp; 
		Triangulator triangluator = new Triangulator (temp); 
		mesh.triangles = triangluator.Triangulate (); 
		mesh.RecalculateNormals (); 
		//this.gameObject.AddComponent<MeshCollider>(mesh);
		MeshCollider mCol = this.gameObject.AddComponent<MeshCollider> (); 
		mCol.sharedMesh = mesh; 
		//mCol.isTrigger = true; 
	}
	
	/*void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("Detected 2D");
		if (other.gameObject.CompareTag ("Unit")) 
		{
			units.Add(other.gameObject); 
		}
	}*/

	public void AddUnit(int faction)
	{
        rgb[faction]++;
		if (rgb [0] > rgb [1] && rgb [0] > rgb [2])
			winner = Affiliation.Red;
		else if (rgb [1] > rgb [0] && rgb [1] > rgb [2])
			winner = Affiliation.Green;
		else if (rgb [2] > rgb [0] && rgb [2] > rgb [1])
			winner = Affiliation.Blue;
		else
			winner = Affiliation.None; 

		//Debug.Log (winner); 
	}
	
}
