using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      SceneManagementManager.Instance.LoadLevel(SceneList.ONLINE, (levelName) => { });
    }

    
}
