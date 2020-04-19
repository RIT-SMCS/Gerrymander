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
        unitMaterials[(int)Affiliation.Yellow] = new Color(243.0f / 255.0f, 201.0f / 255.0f, 105.0f / 255.0f);
        unitMaterials[(int)Affiliation.Green] =new Color(35.0f / 255.0f, 150.0f / 255.0f, 127.0f / 255.0f);
        unitMaterials[(int)Affiliation.Magenta] = new Color(234.0f / 255.0f, 100.0f / 255.0f, 222.0f / 255.0f);
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
                case Affiliation.Yellow:
                    currentMaterial = unitMaterials[(int)Affiliation.Yellow];
                    break;
                case Affiliation.Green:
                    currentMaterial = unitMaterials[(int)Affiliation.Green];
                    break;
                case Affiliation.Magenta:
                    currentMaterial = unitMaterials[(int)Affiliation.Magenta];
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
            dist.AddUnit(this);
        }
    }
}