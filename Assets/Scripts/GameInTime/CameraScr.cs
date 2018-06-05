using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameInTime
{
    public class CameraScr : MainInGame.CameraScr
    {
        public Text timeField;
        public Text pobedaTimeField;

        public GameObject StartMenu;
        public bool startMenuIsOpened;





        public override void OnStart()
        {
            base.OnStart();

            startMenuIsOpened = true;
        }





        public override void _OpenGameMenu()
        {
            base._OpenGameMenu();

            // красииво ыыыыы, а без этой красоты не работает, даааа))))
            ((GameManager)gm).StopTiming();
        }

        public override void _CloseGameMenu()
        {
            base._CloseGameMenu();

            ((GameManager)gm).StartTiming();
        }

        public void CloseStartMenu()
        {
            startMenuIsOpened = false;
            StartMenu.SetActive(false);
            _CloseTransparentWall();

            ((GameManager)gm).StartTiming();
        }

        public override void ClickTransparentWall()
        {
            if ( startMenuIsOpened )
                return;

            base.ClickTransparentWall();
        }





        public void WriteTime( string time )
        {
            timeField.text = time;
        }

        public override void DeclarePobeda()
        {
            base.DeclarePobeda();
            pobedaTimeField.text = timeField.text;
        }
    }
};
