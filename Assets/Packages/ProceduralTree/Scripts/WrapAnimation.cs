using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralModeling;
public class WrapAnimation : MonoBehaviour
{
    public ProceduralModeling.ProceduralTree tree;

    public float startBias = 0.3f;
    public float endBias = 1f;

    public float startLength = 0.5f;
    public float endLength = 5f;

    public float animDuration = 1.0f;
    
    private IEnumerator WrapRoutine()
    {
        var elapsed = 0f;
        while (elapsed < animDuration)
        {
            tree.Rebuild();
            tree.Data.targetBias = Mathf.Lerp(startBias, endBias, elapsed / animDuration);
            // tree.length = Mathf.Lerp(startLength, endLength, elapsed / animDuration);
            yield return null;
            elapsed += Time.deltaTime;
        } 
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(WrapRoutine());
        }
    }
}
