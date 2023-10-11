//Shady
using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour
{
    [SerializeField] Material materialToReveal;
    [SerializeField] Light lightSource;


    //--------------------


    void Update ()
    {
        if (materialToReveal && lightSource)
        {
            //make Transparent if player is away from object
            //float minDist = 5;
            //float dist = Vector3.Distance(lightSource.transform.position, transform.position);
            //if (dist < minDist)
            //{
                
            //}
            //else
            //{
            //    //GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(0, 0, 0, 0));
            //}
            //Vector3 pos = gameObject.transform.position - lightSource.transform.position;

            materialToReveal.SetVector("_LightPosition", lightSource.transform.position);
            materialToReveal.SetVector("_LightDirection", -lightSource.transform.forward);
            materialToReveal.SetFloat("_LightAngle", lightSource.spotAngle);
        }
    }


    //--------------------


    public void SetLightSource(Light light)
    {
        lightSource = light;
    }
    public void SetMaterialToReveal(Material material)
    {
        materialToReveal = material;
    }
}