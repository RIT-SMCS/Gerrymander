using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class DistrictCollider2 : MonoBehaviour {

	//public PolygonCollider2D area; 
	Mesh mesh;// = new Mesh(); 
	public GameObject[] points;
	
	public List<Unit> units = new List<Unit> (); 
	public int[] rgb;
	public Affiliation winner = Affiliation.None; 
    public int NumUnits = 0;
	new Renderer renderer;
	bool checkUnits = false;

	public GameObject angryPrefab, happyPrefab;

	public DistrictCollider2(GameObject[] points)
	{
		SetCollider (points); 
	}
	
	// Use this for initialization
	void Awake () {
		renderer = this.gameObject.GetComponent<Renderer>();
		mesh = new Mesh (); 
		this.GetComponent<MeshFilter> ().mesh = mesh; 
		rgb = new int[3];
		for (int i = 0; i< rgb.Length; i++)
			rgb [i] = 0;
		winner = Affiliation.None;
	}
	
	// Update is called once per frame
	void Update () {
		if(checkUnits)
		{
			foreach (Unit unit in units)
			{
				Vector3 spawnPosition = unit.gameObject.transform.position;
				bool happy = unit.affiliation == winner;
				if (happy)
				{
					GameObject unitFeeling = Instantiate(happyPrefab);
					unitFeeling.transform.position = spawnPosition + Vector3.up * 20;
				} else
				{
					GameObject unitFeeling = Instantiate(angryPrefab);
					unitFeeling.transform.position = spawnPosition + Vector3.up * 20;
				}
			}
			checkUnits = false;
		}
	}
	
	/// <summary>
	/// Takes in an array of gameobjects, 
	/// strips out the vector3s, and passes
	/// the result to SetCollider(Vector3[])
	/// </summary>
	/// <param name="points">Array of GameObjects.</param>
	public void SetCollider(GameObject[] points)
	{
		Vector3 pos = Vector3.zero;
		//Debug.Log ("SetCollider called"); 
		Vector3[] vertices = new Vector3[points.Length]; 
		for (int i = 0; i < points.Length; i++) 
		{
			vertices[i] = points[i].transform.position;
			pos += points[i].transform.position;
		}
		
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
		Vector2[] temp = new Vector2[vertices.Length - 1];
		Vector3 tempTemp; 
		for (int i = 0; i < vertices.Length - 1; i++) {
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
		this.transform.position = this.transform.position - new Vector3(0.0f,1.0f,0.0f); 
		//mCol.isTrigger = true; 
	}

	/// <summary>
	/// This method for calculating the winner of a district. 
	/// Runs once for each member in a district.
	/// </summary>
	/// <param name="faction">Faction.</param>
	public void AddUnit(int faction)
	{
        ++NumUnits;
        rgb[faction]++;
		if (rgb[0] > rgb [1] && rgb [0] > rgb [2]) {
			winner = Affiliation.Yellow;
			renderer.material.color = new Color(243.0f / 255.0f, 201.0f / 255.0f, 105.0f / 255.0f);
        } else if (rgb[1] > rgb [0] && rgb [1] > rgb [2]) {
			winner = Affiliation.Green;
			renderer.material.color = new Color(35.0f / 255.0f, 150.0f / 255.0f, 127.0f / 255.0f);
        } else if (rgb [2] > rgb [0] && rgb [2] > rgb [1]) {
			winner = Affiliation.Magenta;
			renderer.material.color = new Color(234.0f / 255.0f, 100.0f / 255.0f, 222.0f / 255.0f);
        } else {
			winner = Affiliation.None;
			renderer.material.color = Color.grey; 
		}
		checkUnits = true;
	}
	

	public void AddUnit(Unit unit)
	{
		++NumUnits;
		units.Add(unit);
		int faction = (int)unit.affiliation;
		rgb[faction]++;
		if (rgb[0] > rgb[1] && rgb[0] > rgb[2])
		{
			winner = Affiliation.Yellow;
			renderer.material.color = new Color(243.0f / 255.0f, 201.0f / 255.0f, 105.0f / 255.0f);
		}
		else if (rgb[1] > rgb[0] && rgb[1] > rgb[2])
		{
			winner = Affiliation.Magenta;
			renderer.material.color = new Color(234.0f / 255.0f, 100.0f / 255.0f, 222.0f / 255.0f);
		}
		else if ( rgb[2] > rgb[0] && rgb[2] > rgb[1])
		{
			winner = Affiliation.Green;
			renderer.material.color = new Color(35.0f / 255.0f, 150.0f / 255.0f, 127.0f / 255.0f);
		}
		else 
		{
			winner = Affiliation.None;
			renderer.material.color = Color.grey;
		}
		checkUnits = true;
	}
}
