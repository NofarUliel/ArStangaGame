using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField]
    private Toggle checkMarkBtn;
    private void Start()
    {
        checkMarkBtn.isOn = CameraPermission.isCameraPermssion; 
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetPermission()
    {
        bool isArCameraMark = checkMarkBtn.isOn;
        if (isArCameraMark) //ar camera on
        {
            CameraPermission.isCameraPermssion = true;

        }
        else //ar camera OFF
        {
            CameraPermission.isCameraPermssion = false;

        }

    }
}
