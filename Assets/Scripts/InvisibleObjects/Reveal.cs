//Shady
using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour
{
    [SerializeField] Material materialToReveal;
    [SerializeField] Light lightSource;


    [SerializeField] Material on_Material;
    [SerializeField] Material off_Material;

    //--------------------


    void Update ()
    {
        //if (materialToReveal && lightSource)
        //{
        //    //make Transparent if player is away from object
        //    //float minDist = 5;
        //    //float dist = Vector3.Distance(lightSource.transform.position, transform.position);
        //    //if (dist < minDist)
        //    //{
                
        //    //}
        //    //else
        //    //{
        //    //    //GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(0, 0, 0, 0));
        //    //}
        //    //Vector3 pos = gameObject.transform.position - lightSource.transform.position;

        //    materialToReveal.SetVector("_LightPosition", lightSource.transform.position);
        //    materialToReveal.SetVector("_LightDirection", -lightSource.transform.forward);
        //    materialToReveal.SetFloat("_LightAngle", lightSource.spotAngle);
        //}
    }


    //--------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Light")
        {
            GetComponent<MeshRenderer>().material = on_Material;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Light")
        {
            GetComponent<MeshRenderer>().material = off_Material;
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