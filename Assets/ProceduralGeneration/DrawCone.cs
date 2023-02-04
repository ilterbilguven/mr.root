using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCone : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float angle = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        var p1 = target.position;
        var p2 = transform.position;
        Gizmos.DrawLine(p1, p2);
        const float step = 1f;
        var l = (p2- p1).magnitude;
    }
}
