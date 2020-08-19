using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar
{
    public const string AI_AVATAR = "aiAvatar";
    private Sprite avatar1, avatar2, avatar3, avatar4, avatar5, avatar6,aiAvatar, defualtAvatar;
    public Avatar()
    {
        avatar1 = Resources.Load<Sprite>("Avatar/Avatar1");
        avatar2 = Resources.Load<Sprite>("Avatar/Avatar2");
        avatar3 = Resources.Load<Sprite>("Avatar/Avatar3");
        avatar4 = Resources.Load<Sprite>("Avatar/Avatar4");
        avatar5 = Resources.Load<Sprite>("Avatar/Avatar5");
        avatar6 = Resources.Load<Sprite>("Avatar/Avatar6");
        aiAvatar= Resources.Load<Sprite>("Avatar/computerAvatar");
        defualtAvatar = Resources.Load<Sprite>("Avatar/Avatar0");
    }

    public void DisplayAvatar(Image avatar, string nameAvatar)
    {
        switch (nameAvatar)
        {
            case "Avatar1":
                avatar.sprite = avatar1;
                break;
            case "Avatar2":
                avatar.sprite = avatar2;
                break;
            case "Avatar3":
                avatar.sprite = avatar3;
                break;
            case "Avatar4":
                avatar.sprite = avatar4;
                break;
            case "Avatar5":
                avatar.sprite = avatar5;
                break;
            case "Avatar6":
                avatar.sprite = avatar6;
                break;
            case AI_AVATAR:
                avatar.sprite = aiAvatar;
                break;
            default:
                avatar.sprite = defualtAvatar;
                break;

        }

    }
}

