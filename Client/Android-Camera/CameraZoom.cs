using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
    private Vector3 touchStart;
    private float zoomOutMin = 1;
    private float zoomOutMax = 8;
    [SerializeField]
    private Camera playerCamera;
    private bool isCanZoom;
    private void Start()
    {
        isCanZoom = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MainMenuManager.currentLevelGame != null)
        {
            if (MainMenuManager.currentLevelGame.Equals(SceneList.MULTIPLAYER_LEVEL1) || MainMenuManager.currentLevelGame.Equals(SceneList.MULTIPLAYER_LEVEL2))
            {
                if ((!PlayerManager.isJoystickDrage) && (!MultiplayerManager.isArCameraOn) && (!PlayerManager.isKick))
                {
                    isCanZoom = true;
                }
                else { isCanZoom = false; }
            }
            else if (MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL1) || MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL2))
            {
                if ((!HumanPlayer.isJoystickDrage) && (!GameController.isArCameraOn) && (!HumanPlayer.isKick))
                {
                    isCanZoom = true;
                }
                else
                {
                    isCanZoom = false;
                }
            }
        }

        if (isCanZoom)
        {


            if (Input.GetMouseButtonDown(0))
            {
                touchStart = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            }
#if PLATFORM_ANDROID

            if (Input.touchCount == 2)//zoom with touch ->two fingers touch the screen
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }
#endif
            else if (Input.GetMouseButton(0))//zoom with scrole mouse
            {

                Vector3 direction = touchStart - playerCamera.ScreenToWorldPoint(Input.mousePosition);
                playerCamera.transform.position += direction;
            }
            zoom(Input.GetAxis("Mouse ScrollWheel"));

        }
    }


    void zoom(float increment)
    {
        playerCamera.orthographicSize = Mathf.Clamp(playerCamera.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}
