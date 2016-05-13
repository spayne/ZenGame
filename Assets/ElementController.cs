using UnityEngine;
using System.Collections;

public class ElementController : MonoBehaviour {

    public char pipe_type;
    public Renderer rend;

    private float val;
    private int count;
    
        void Start()
        {
            rend = GetComponent<Renderer>();        
        }

    void Update()
    {
        Material m = rend.material;
        if (count++ % 60 == 0)
        {
            val = Random.value;
            m.SetFloat("_Threshy", val);
        }
        
       
    }
}
