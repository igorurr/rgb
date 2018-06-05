using System;
using System.Collections;
using System.Collections.Generic;
using MainInGame;
using UnityEngine;

namespace GameInTime
{
    public class GameManager : MainInGame.GameManager
    {
        /*
         *      Работает по принципу:
         *      в StartTiming в oldDate записываем текущий timestamp
         *      в UpdateTiming в newDate записываем текущий timestamp, находим разницу между newDate и oldDate, добавляем в bankTime, в oldDate записываем newDate
         *      в StopTiming останавливаем запись
         * */
        public bool timeWriting;
        public int bankTime;
        public int oldDate;



        public override void OnStart()
        {
            bankTime = 0;
            ((CameraScr)cam).WriteTime(TimestampToString(bankTime));
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateTiming();
        }

        

        public override void DeclarePobeda(Point point, int whoPobeditel)
        {
            base.DeclarePobeda(point, whoPobeditel);

            StopTiming();

            int recordTime = (countPlayers == 2) ? PlayerPrefs.GetInt("_rec_GameInTime2", -1) : PlayerPrefs.GetInt("_rec_GameInTime3", -1);
            if ( bankTime < recordTime  ||  recordTime == -1 )
            {
                if (countPlayers == 2)
                    PlayerPrefs.SetInt("_rec_GameInTime2", bankTime);
                else
                    PlayerPrefs.SetInt("_rec_GameInTime3", bankTime);
            }
        }



        public void StartTiming()
        {
            if( win )
                return;

            timeWriting = true;
            oldDate = GetCurentTimestamp();
        }

        public void UpdateTiming()
        {
            if (!timeWriting)
                return;

            int newDate = GetCurentTimestamp();
            bankTime += newDate - oldDate;
            oldDate = newDate;
            
            ((CameraScr)cam).WriteTime( TimestampToString(bankTime) );
        }

        public void StopTiming()
        {
            timeWriting = false;
        }



        public int GetCurentTimestamp()
        {
            return DateTimeToUInt64(DateTime.UtcNow);
        }
        public int DateTimeToUInt64( DateTime dt )
        {
            return (
                dt.Millisecond +
                dt.Second * 1000 +
                dt.Minute * 60 * 1000 +
                dt.Hour * 60 * 60 * 1000 +
                dt.Day * 24 * 60 * 60 * 1000
            );
        }

        public static string TimestampToString( int dt )
        {
            int d = (int)dt / (24 * 60 * 60 * 1000);
            dt %= 24 * 60 * 60 * 1000;

            int h = (int)dt / (60 * 60 * 1000);
            dt %= 60 * 60 * 1000;

            int m = (int)dt / (60 * 1000);
            dt %= 60 * 1000;

            int s = (int)dt / 1000;

            int msi = (int)dt % 1000;
            string ms = msi.ToString();
            if( ms.Length==1 )
                ms = "00" + ms;
            else if( ms.Length==2 )
                ms = "0" + ms;
            
            if ( m == 0 )
                return string.Format("{0}.{1}сек", s, ms);
            else if ( h == 0 )
                return string.Format("{0}м {1}.{2}сек", m, s, ms);
            else if ( d == 0 )
                return string.Format("{0}ч {1}м {2}.{3}сек", h, m, s, ms);
            else
                return string.Format("{0}д {1}ч {2}м {3}.{4}сек", d, h, m, s, ms);
        }
    }
};