using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Unit : MonoBehaviour {
    public Affiliation affiliation;
    private Affiliation oldAffiliation;

    Color[] unitMaterials = new Color[3];
    new SpriteRenderer renderer;
    Color currentMaterial = Color.white;
    // Use this for initialization
    void Awake()
    {
        unitMaterials[(int)Affiliation.Red] = new Color(243.0f / 255.0f, 201.0f / 255.0f, 105.0f / 255.0f);
        unitMaterials[(int)Affiliation.Green] =new Color(35.0f / 255.0f, 150.0f / 255.0f, 127.0f / 255.0f);
        unitMaterials[(int)Affiliation.Blue] = new Color(64.0f / 255.0f, 121.0f / 255.0f, 140.0f / 255.0f);
        //unitMaterials[(int)Affiliation.None] = MaterialSetup(new Color(255.0f / 255.0f, 81.0f / 255.0f, 98.0f / 255.0f));
        renderer = this.GetComponent<SpriteRenderer>(); 
        //renderer.sharedMaterials = unitMaterials;
    }
	
	// Update is called once per frame
	void Update () {
        if (affiliation != oldAffiliation)
        {
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
            renderer.color = currentMaterial;
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

