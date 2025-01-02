using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Pixel_Backgrounds_2
{
    public class CameraController : MonoBehaviour
    {
        public float camSpeed = 5f;
        public float minX = 0f;
        public float maxX = 15f;
        public Toggle Automove;
        public bool AM;

        private float camPos;


        void Start()
        {
            AM = false;
            camPos = transform.position.x;
            Automove.isOn = false;
            QualitySettings.vSyncCount = 4;
        }
        void Update()
        {
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && transform.position.x < maxX)
            {
                transform.Translate(new Vector3(camSpeed * Time.deltaTime, 0, 0));
                Automove.isOn = false;
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && transform.position.x > minX)
            {
                transform.Translate(new Vector3(-camSpeed * Time.deltaTime, 0, 0));
                Automove.isOn = false;
            }
            if (Automove.isOn && transform.position.x < maxX || AM)
            {
                transform.Translate(new Vector3(camSpeed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                VideoMove();
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                VideoNextLevel();
            }
        }
        public void AutomoverCheck()
        {
            if (Automove.isOn == false)
            {
                Automove.isOn = true;
            }
            else
            {
                Automove.isOn = false;
            }
        }
        public void VideoMove()
        {
            AM = true;
        }
        void VideoNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

