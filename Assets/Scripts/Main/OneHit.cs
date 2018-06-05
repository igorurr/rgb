using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MainInGame
{
    public class OneHit : MonoBehaviour {
	    public static Grid grd;
        public static GameManager gm;
        public BoCoCell bcc;   // задаётся в инспекторе юнити

        public int state;			// state: -1:неактивно 0:пусто 1:синий 2:красный 3:зелёный 4синийWin 5:крассныйWin
	    public int whoShodil		// кто сходил в данную клетку? 0-никто 1-синий 2-красный 3-зелёный
		{get{return (state>0) ? (state-1)%3+1 : 0;}}
		
	    public Point coord; // Трёхмерные координаты данной точки
        //public OneHit[] around; От этой штуки пришлось отказаться. Невозможно инициализировать значения этой переменной на старте. Плюс мы ищем точки не только вокруг данной, при создании точек требуется поиск среди всех

        // Мышка была нажата, потенциально можем сделать ход в ячейку,
        // если она не покинула ячейку, как только покинет будет false
        public bool mouseDidDown;

        /*
            *  Жизненный цикл объекта OneHit, при создании приобретает указаные координаты и состояние 0
            *  После хода в точку данного объекта меняет состояние в зависимости от того кто сходил
            *  Если победная линия проходит через данную точку, приобретает победный префаб и меняет угол
            * */
	    void Start()
        {
            // не работай здесь через старт вообще - нахуй его, реализовывай Create и вызывай там где надо
            // start работает в другом потоке походу
        }
        public void Initialize( Point coord ){
		    this.coord = coord;
            SetState(0);
        }





	    public void SetState( int newState ) {
		    GetComponent<SpriteRenderer>().sprite = grd.cellPrefabs[newState];
		    state = newState;
            
            //newState:whoShodil countPlayers=3  -1:0  0:0  1:1  2:2  3:3  4:1 5:2 6:3
            //newState:whoShodil countPlayers=2  -1:0  0:0  1:1  2:2  3:1  4:2
            //whoShodil = (newState > 0) ? (newState-1)%grd.gm.countPlayers+1 : 0; Более не требует обновления
	    }

        public void SetStateWin( int rotation )
        {
            // тут всегда +3 независимо от кол-ва игроков, т.к. массив префабов ячеек всегда статичен и не зависит от кол-ва игроков.
            state += 3;
            GetComponent<SpriteRenderer>().sprite = grd.cellPrefabs[state];

            //  0:0  1:-60  2:-120  3:0  4:-60  5:-120
            rotation = (rotation % 3) * -60;
            transform.rotation *= Quaternion.Euler(0, 0, rotation);
        }



	    public void OnMouseDown() {
		    if ( grd.gm.cam.CheckClickInUI() )
			    return;
			
		    mouseDidDown = true;
	    }
	    public void OnMouseExit() {
		    if (!mouseDidDown)
			    return;

            grd.gm.cam.StartMovie();

		    mouseDidDown = false;
	    }
	    public void OnMouseUp() {
            //Debug.Log (string.Format ( "x:{0} y:{1} z:{2} st:{3} pr:", coord.x, coord.y, coord.z, state, pref.ToString() ));
            if ( mouseDidDown )
                grd.gm.MakeHitLocalPlayer( this );

            grd.gm.cam.StopMovie();

		    mouseDidDown = false;
	    }
    }
};