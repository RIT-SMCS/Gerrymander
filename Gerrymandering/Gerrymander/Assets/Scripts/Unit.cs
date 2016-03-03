using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Unit : MonoBehaviour {
    public Affiliation affiliation;
    private Affiliation oldAffiliation;

    Material[] unitMaterials = new Material[3];
    new Renderer renderer;
    Material currentMaterial = null;
    // Use this for initialization
    void Awake()
    {
        unitMaterials[(int)Affiliation.Red] = MaterialSetup(new Color(255.0f / 255.0f, 81.0f / 255.0f, 98.0f / 255.0f), "red");
        unitMaterials[(int)Affiliation.Green] = MaterialSetup(new Color(94.0f / 255.0f, 255.0f / 255.0f, 134.0f / 255.0f), "green");
        unitMaterials[(int)Affiliation.Blue] = MaterialSetup(new Color(74.0f / 255.0f, 94.0f / 255.0f, 232.0f / 255.0f), "blue");
        //unitMaterials[(int)Affiliation.None] = MaterialSetup(new Color(255.0f / 255.0f, 81.0f / 255.0f, 98.0f / 255.0f));
        renderer = this.GetComponent<Renderer>(); 
        //renderer.sharedMaterials = unitMaterials;
    }
	
	// Update is called once per frame
	void Update () {
        if (affiliation != oldAffiliation)
        {
            if (currentMaterial != null)
            {
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(currentMaterial));
            }
            switch (affiliation)
            {
                case Affiliation.Red:
                    currentMaterial = unitMaterials[(int)Affiliation.Red];
                    break;
                case Affiliation.Green:
                    currentMaterial = unitMaterials[(int)Affiliation.Green];
                    break;
                case Affiliation.Blue:
                    currentMaterial = unitMaterials[(int)Affiliation.Blue];
                    break;
            }
            renderer.material = currentMaterial;
        }
        oldAffiliation = affiliation;
	}

    private Material MaterialSetup(Color c, string name)
    {
        Material m = new Material(Shader.Find("Diffuse"));
        m.SetColor("_Color", c);
        m.name = name;
        return m;
    }

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("District")) 
		{
			DistrictCollider2 dist = other.gameObject.GetComponent<DistrictCollider2>(); 
			if(affiliation == Affiliation.Red)
				dist.AddUnit(0); 
			else if(affiliation == Affiliation.Green)
				dist.AddUnit(1); 
			else
				dist.AddUnit(2); 
		}
	}
}

