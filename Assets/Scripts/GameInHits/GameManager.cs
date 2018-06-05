using System.Collections;
using System.Collections.Generic;
using MainInGame;
using UnityEngine;

namespace GameInHits
{
    public class GameManager : MainInGame.GameManager
    {
        public int hits;



        public override void OnStart()
        {
            base.OnStart();

            hits = 0;
            ((CameraScr)cam).CountHitsView();
        }

        public override void DeclarePobeda(Point point, int whoPobeditel)
        {
            base.DeclarePobeda(point, whoPobeditel);

            int recordhits = (countPlayers==2) ? PlayerPrefs.GetInt("_rec_GameInHits2", -1) : PlayerPrefs.GetInt("_rec_GameInHits3", -1);
            if ( hits < recordhits || recordhits == -1 )
            {
                if (countPlayers == 2)
                    PlayerPrefs.SetInt("_rec_GameInHits2", hits);
                else
                    PlayerPrefs.SetInt("_rec_GameInHits3", hits);
            }
        }
    }
};