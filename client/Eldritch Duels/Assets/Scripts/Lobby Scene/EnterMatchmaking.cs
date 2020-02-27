using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMatchmaking : MonoBehaviour
{

    public UnityEngine.UI.Button enter;
    // Start is called before the first frame update
    void Start()
    {
        enter.onClick.AddListener(clicked);
    }

    void clicked()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
