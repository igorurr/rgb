using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MainInGame
{
    public class GameManager : MonoBehaviour
    {
        public CameraScr cam;
        public Grid grd;

        public int countPlayers;        // Количество всех игроков в катке. И боты и игроки локалки и мультиплееры, все. 2 или 3
        public int countRealPlayers;    // Количество живых игроков в игре. Локалка или мультиплеер
        public int countBots;     	  // Без комментариев.

        public BotController[] bcs;     // массив ботов
        public InternetPlayer[] ips;     // массив интернет игроков

        // Порядок ходов. На i позиции надится тот кто должен ходить iым, если это бот - указатель на InternetPlayer == null, и наоборот.
        public Typple2<BotController,InternetPlayer>[] orderHits;

        public int modeGame;    // Основной режим игры:   0 - против ботов (все боты)   1 - локалка(м.б.+бот)   2 - интернет(м.б.+бот)

        public int playerCurHit;    // 0 1 2

        public bool win;        // была ли победа



        // Use this for initialization
        void Start () {
            /*Trie trr = new Trie("dsd", new OneHit());
            trr.Insert("dsdf", new OneHit());
            trr.Insert("dsd", new OneHit());
            trr.Insert("ds", new OneHit());
            trr.Insert("op", new OneHit());
            //trr.Insert("dsdf", new OneHit());*/


            BotController.gm = this;
            OneHit.gm = this;
            BoCoCell.gm = this;
            BoCoCell.bcs = bcs;

            modeGame = LocalDB._def_ModeGame;
            countPlayers = LocalDB._def_CountPlayers;

            orderHits = new Typple2<BotController, InternetPlayer>[countPlayers];

            playerCurHit = 0;

            win = false;

            if ( modeGame==0 )
            {   // гарантированно 1 игрок, и либо 2 либо 3 бота
                countRealPlayers = 1;
                countBots = countPlayers - 1;

                bcs = new BotController[countBots];
                ips = null;

                for(int i=0,j=0; i<countPlayers; i++)
                {
                    // номер цвета соответствует номеру хода по порядку
                    // i+1 потому что цвет: 1 2 3, а i: 0 1 2
                    if( LocalDB._def_ColorPlayer1 != i+1 ) { 
                        bcs[j] = new BotController( i+1 );
                        orderHits[i] = new Typple2<BotController, InternetPlayer>(bcs[j],null);
                        j++;
                    }
                    else 
                        orderHits[i] = new Typple2<BotController, InternetPlayer>(null,null);
                }
            }
            else if( modeGame==1 )
            {
                if( countPlayers == 2 )
                {
                    countRealPlayers = 2;
                    countBots = 0;

                    bcs = null;
                }
                else // countPlayers==3
                {
                    countRealPlayers = LocalDB._def_CountRealPlayers;
                    countBots = countPlayers - countRealPlayers;

                    bcs = (countBots > 0) ? new BotController[countBots] : null;
                
                    // a,b:6-a-b   1,2:3   1,3:2   2,3:1     Эта штука нужна для определения хода бота, либо оставшегося игрока
                    int lastPlayer = 6-LocalDB._def_ColorPlayer1-LocalDB._def_ColorPlayer2;
                    if( countBots == 1 ) { 
                        bcs[0] = new BotController(lastPlayer);
                        orderHits[lastPlayer-1] = new Typple2<BotController, InternetPlayer>( bcs[0], null );
                    }
                    else
                        orderHits[lastPlayer-1] = new Typple2<BotController, InternetPlayer>( null, null );
                }
                ips = null;

                // -1 потому что синему должен соответствовать 1 элемент а красному 2, в реальности элементы массива отсчитываются с 0
                orderHits[LocalDB._def_ColorPlayer1-1] = new Typple2<BotController, InternetPlayer>(null, null);
                orderHits[LocalDB._def_ColorPlayer2-1] = new Typple2<BotController, InternetPlayer>(null, null);
            }
            else // gameType==2
            {/*
                if (countPlayers == 2)
                {
                    countRealPlayers = 2;
                    countBots = 0;

                    bcs = null;
                    ips = new InternetPlayer[1];
                }
                else // countPlayers==3
                {
                    countRealPlayers = LocalDB._def_CountRealPlayers;
                    countBots = countPlayers - countRealPlayers;

                    bcs = (countBots>0) ? new BotController[countBots] : null;
                    ips = new InternetPlayer[countRealPlayers-1];
                }*/
            }
            
            BoCoCell.bcs = bcs;

            GetHit();

            OnStart();
        }
	
	    // Update is called once per frame
	    void Update () {
            OnUpdate();
	    }



        /*
         *      Если ходет живой чел за данным телефоном - вызывается MakeHitLocalPlayer,
         *      если ходит бот или чувак по интернетику - GetHit
         *      после всех этих действий вызывается UpdateAllAfterHit для обновления префабов, вызовов реанализаторов ботов и прочего.
         *      
         *      После хода живого чела бесконечной рекурсией вызывается GetHit, где ходит бот или интернет чувак, в зависимости от того что в orderHits[i] обозначено как null,
         *      рекурсия разравается когда должен ходить локальный игрок
         *      UpdateAllAfterHit() вызывается после каждого хода.
         * */

        // Это работает для всех режимов игры. и мультиплеер и против ботов. Локальный игрок - это тот кто может ходить с устройства и не является ботом.
        public void MakeHitLocalPlayer( OneHit p )
        {
            if ( !MakeHit( p, playerCurHit+1 ) )
                return;

            if( !win )
                UpdateAllAfterHit(p);
            else
                return;

            GetHit();
        }

        // Для ботов и мультиплеерных игроков
        public void GetHit()
        {
            return;
            if( orderHits[playerCurHit].x1==null && orderHits[playerCurHit].x2 == null )
                return;     // Долен ходить локальный игрок

            OneHit p;

            if ( orderHits[playerCurHit].x1 != null )
            {   // ходит бот
                BotController bc = orderHits[playerCurHit].x1;
                p = bc.ScorePointsGetTop();
                MakeHit(p, playerCurHit+1);

                if (!win)
                    UpdateAllAfterHit(p);
            }
            else
            {   // очевидно - ход игрока в инетике

            }
            

            GetHit();
        }

        public void UpdateAllAfterHit( OneHit p )
        {
            // Боты перерасчитывают свои веса
            for(int i=0; i<countBots; i++)
            {
                bcs[i].ReanalyseAfterHit(p);
            }

            // Определяем кто ходит
            // playerCurHit:playerCurHit   0:1 1:2 2:0    или 0:1 1:0
            playerCurHit = (playerCurHit+1) % countPlayers;

            // в камере меняем иконку игрока который ходит
            cam.UpdateImgPlayerCurHit();
        }

        public virtual bool MakeHit(OneHit point, int player)
        {
            return grd.MakeHit( point, player );
        }

        public virtual void DeclarePobeda(Point point, int whoPobeditel)
        {
            win = true;
            cam.DeclarePobeda();
        }



        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
};