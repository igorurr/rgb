using System.Collections;
using System.Collections.Generic;
using MainInGame;
using UnityEngine;

namespace GameInHits
{
    public class Grid : MainInGame.Grid
    {
        public override bool MakeHit(MainInGame.OneHit point, int player)
        {
            if (point.state != 0 || gm.win == true)
                return false;
            
            // Если сходил синий - необходимо инкриментировать количество ходов
            if (((GameManager)gm).playerCurHit == 0)
            {
                ((GameManager)gm).hits++;
                // согласен, забавная конструкция, но если её убрать, он будет брать GameManager из MainInGame, а не из GameInHits
                ((CameraScr)gm.cam).CountHitsView();
            }

            if (!base.MakeHit(point, player))
                return false;

            return true;
        }
    }
};