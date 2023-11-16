using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Backspace))
         {

             var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
             var type = assembly.GetType("UnityEditor.LogEntries");
             var method = type.GetMethod("Clear");
             method.Invoke(new object(), null);

         }
            */

    }
}
