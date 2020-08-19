using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventFall : MonoBehaviour
{
    private float groundPos;
    // Start is called before the first frame update
    void Start()
    {
        switch (MainMenuManager.currentLevelGame)
        {
            case SceneList.PLAYER_VS_COMPUTER_LEVEL1:
                groundPos = 0.3f;
                break;
            case SceneList.PLAYER_VS_COMPUTER_LEVEL2:
                groundPos = -2.3f;
                break;
            case SceneList.MULTIPLAYER_LEVEL1:
                groundPos = 0.5f;
                break;
            case SceneList.MULTIPLAYER_LEVEL2:
                groundPos = 0.72f;
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y < groundPos)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, groundPos, this.transform.localPosition.z);

        }
        if (this.transform.localPosition.y > groundPos && this.tag.CompareTo("ai")==0)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, groundPos, this.transform.localPosition.z);

        }
    }
}
