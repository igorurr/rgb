using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameInHits
{
    public class CameraScr : MainInGame.CameraScr
    {
        public Text hitsField;
        public Text pobedaHitsField;


        public void CountHitsView()
        {
            // согласен, забавная конструкция, но если её убрать, он будет брать GameManager из MainInGame, а не из GameInHits
            hitsField.text = "ходов: " + ((GameManager)gm).hits.ToString();
        }

        public override void DeclarePobeda()
        {
            base.DeclarePobeda();
            pobedaHitsField.text = hitsField.text;
        }
    }
};
