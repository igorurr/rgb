using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MainInGame
{
    public class Grid : MonoBehaviour
    {
        public GameManager gm;

        private Dictionary<string, OneHit> points;

        // cellPrefabs: 0:пусто 1:синий 2:красный 3:зелёный 4:синийWin 5:крассныйWin 6:зелёныйWin
        public List<Sprite> cellPrefabs;

        public List<GameObject> lastHitedPoint;

        // Эталон ячеек
        public GameObject mainCell;

        public int winCells;     // Сколько надо ячеек выстроить в линию чтобы победить
        public int blackCells;  // толщина пустых ячеек вокруг занятых

        private void Start()
        {
            OneHit.grd = this;
            BoCoCell.grd = this;
            points = new Dictionary<string, OneHit>();

            // 2:5 3:4
            winCells = 7 - LocalDB._def_CountPlayers;

            // Создаём новую точку в 000 координатах и получаем её объект на сцене
            Point zeroPoint = new Point(0, 0);
            OneHit newCellElem = NewCell(zeroPoint);

            // заполняем полупрозрачное поле
            AddBlackField(zeroPoint);

            OnStart();
        }
        private void Update()
        {
            OnUpdate();
        }

        private void AddBlackField(Point point)
        {
            for (int x = -blackCells; x <= blackCells; x++)
                for (int y = -blackCells; y <= blackCells; y++)
                {
                    // В 3мерных координатах 2мерной сетки сумма трёх координат должна обнуляться,
                    // иначе возникнет левая ячейка которой не должно быть
                    // x+y+z = 0 = x+y-(x+y)
                    // z = -(x + y);
                    if (Math.Abs(x + y) > blackCells)
                        continue;

                    Point curPoint = new Point(point.x + x, point.y + y);
                    if (FindPoint(curPoint) == null)
                    {
                        NewCell(curPoint);
                    }

                }
        }

        public OneHit NewCell(Point point)
        {
            GameObject newEl = Instantiate(mainCell, point.GetCoord2D(), Quaternion.identity);
            newEl.name = point.ToString();
            newEl.transform.SetParent(transform);

            OneHit ohp = newEl.GetComponent<OneHit>();

            ohp.Initialize(point);
            points.Add(newEl.name, ohp);

            return newEl.GetComponent<OneHit>();
        }

        // совершить ход в данную точку. Финальная - завершительная часть хода.
        public virtual bool MakeHit(OneHit point, int player)
        {
            //player: 1 2 3
            // проверить занята ли точка, либо случилась победа
            if (point.state != 0 || gm.win == true)
                return false;

            // Обновить точку, выше всех остальных шагов чтобы пользователь видел что произошёл ход
            point.SetState(player);

            // В последнюю схоженную данным игроком ячейку помещаем "иконку последнего хода" этого игрока
            lastHitedPoint[player - 1].transform.position = point.transform.position;

            if (FindPobedaInPoint(point.coord, player))
            {
                return true;
            }

            // создать чёрное поле
            AddBlackField(point.coord);

            UnityEngine.Debug.Log(string.Format("{0}", points.Count));

            return true;
        }



        public OneHit FindPoint(Point point)
        {
            return FindPointStr(point.ToString());
        }
        // Есть куски кода где надо искать по строке а не по точке
        public OneHit FindPointStr(string pstr)
        {
            if (!points.ContainsKey(pstr))
                return null;

            return points[pstr];
        }



        // Найдём же победу в данной точке
        private bool FindPobedaInPoint(Point point, int whoHited)
        {
            List<short> countPointsWhoHitedInSubLine = new List<short>();

            // отдельно рассмотрим каждое направление
            for (short line = 0; line < 6; line++)
            {
                Point curPoint = point.GetAround(line);
                OneHit curPointOH = FindPoint(curPoint);
                short i = 0;

                // такая ситуация может появиться если ход был на границе
                if( curPointOH != null )
                    for (; curPointOH.state == whoHited; i++)
                    {
                        curPoint = curPoint.GetAround(line);
                        curPointOH = FindPoint(curPoint);
                    }

                countPointsWhoHitedInSubLine.Add(i);
                if (line >= 3)
                {
                    if (countPointsWhoHitedInSubLine[line - 3] + countPointsWhoHitedInSubLine[line] + 1 >= winCells)
                    {
                        gm.DeclarePobeda(point, whoHited);
                        ReColorCellsPobeda(FindPoint(point), whoHited, line);
                        return true;
                    }
                }
            }

            return false;
        }

        // перекрасить ячейки победившей линии в победные ячейки
        private void ReColorCellsPobeda(OneHit point, int whoHited, short line)
        {
            point.SetStateWin(line);

            Point curPoint = point.coord.GetAround(line);
            OneHit curPointOH = FindPoint(curPoint);
            short i = 0;
            for (; curPointOH.state == whoHited; i++)
            {
                curPointOH.SetStateWin(line);
                curPoint = curPoint.GetAround(line);
                curPointOH = FindPoint(curPoint);
            }

            //  0:3  1:4  2:5  3:6  4:1  5:2
            line = (short)((line + 3) % 6);

            curPoint = point.coord.GetAround(line);
            curPointOH = FindPoint(curPoint);
            i = 0;
            for (; curPointOH.state == whoHited; i++)
            {
                curPointOH.SetStateWin(line);
                curPoint = curPoint.GetAround(line);
                curPointOH = FindPoint(curPoint);
            }
        }






        public Vector2 GetCoordLastPoint()
        {
            // Если сейчас ходит синий - берём позицию красного, т.к. он ходил последним,
            // но если случилась победа, тогда изменения playerCurHit не происходит и берём то что есть
            int retInt;
            if (gm.win)
                retInt = gm.playerCurHit;
            else if (gm.countPlayers == 3)
                //  0:2  1:0  2:1
                retInt = (gm.playerCurHit + 2) % 3;
            else// if ( countPlayers == 2 )
                //  0:1 1:0
                retInt = (gm.playerCurHit + 1) % 2;

            return lastHitedPoint[retInt].transform.position;
        }






        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
};