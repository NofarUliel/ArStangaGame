using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class CameraPermission : MonoBehaviour
{
    public static bool isCameraPermssion;
    void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            isCameraPermssion = false;
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            isCameraPermssion = false;
            Debug.Log("deny");
            return;
        }
        else
        {
          isCameraPermssion = true;
         // isCameraPermssion = false;

            Debug.Log("Is camera permmision = "+ isCameraPermssion);

        }

#endif
    }

}