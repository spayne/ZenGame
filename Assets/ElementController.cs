using UnityEngine;
using System.Collections;

public class ElementController : MonoBehaviour {

    public char pipe_type;
    public Renderer rend;

    public float fill_ew;
    public float fill_we;
    public float fill_ns;
    public float fill_sn;

    void Start()
        {
            rend = GetComponent<Renderer>();        
        }

    void Update()
    {
        Material m = rend.material;
        m.SetFloat("_FillEW", fill_ew);
        m.SetFloat("_FillWE", fill_we);
        m.SetFloat("_FillNS", fill_ns);
        m.SetFloat("_FillSN", fill_sn);
    }
}
