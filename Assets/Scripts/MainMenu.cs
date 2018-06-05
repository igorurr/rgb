using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public string[][] sceneTxt;

    public List<Sprite> cellPrefabs;    // Префабы синий красный зелёный

    public Text fieldCountPlayers;
    public Text fieldOrientation;  // ориентация шестиугольников на сетке. Реализуется поворотом камеры на 30 градусов. 0-горизонтальная 1-вертикальная
    
    public GameObject mainMenuScn;      // Объект main menu на сцене
    public GameObject recordsScn;      // Объект records на сцене




    public Text fieldRecordInTime2;
    public Text fieldRecordInHits2;
    public Text fieldRecordInTime3;
    public Text fieldRecordInHits3;





    // Use this for initialization
    void Start()
    {
        sceneTxt = new string[6][];

        sceneTxt[0] = new string[3] { "Игрок на Бота", "Игрок на Игрока", "Игрок на Игрока\nпо интернету" };
        sceneTxt[1] = new string[4] { "Игрок на Ботов", "Игрок на Бота и Игрока", "Игрок на Игроков", "Игрок на Игроков\nпо интернету" };
        sceneTxt[2] = new string[2] { "Горизонтальная ориентация", "Вертикальная ориентация" };
        sceneTxt[3] = new string[2] { "Игра на победу", "Игра на поражение" };

        SetCountPlayers();

        SetOrientation();

        CheckOpenedRecords();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void SetCountPlayers()
    {
        fieldCountPlayers.text = LocalDB._def_CountPlayers.ToString();
    }
    
    



    public void SwapCountPlayers()
    {
        LocalDB._def_CountPlayers = (LocalDB._def_CountPlayers == 3) ? 2 : 3;
        LocalDB._def_CountRealPlayers = LocalDB._def_CountPlayers;


        SetCountPlayers();
    }

    
    



    // Просто берём и меняем
    public void SwapOrientation()
    {
        LocalDB._def_GexagonsOrientation = (LocalDB._def_GexagonsOrientation == 1) ? 0 : 1;
        SetOrientation();
    }
    private void SetOrientation()
    {
        fieldOrientation.text = (LocalDB._def_GexagonsOrientation == 0) ? sceneTxt[2][0] : sceneTxt[2][1];
    }





    public void NewNormalGame()
    {
        SceneManager.LoadScene("GameNormal");
    }
    public void NewGameInTime()
    {
        SceneManager.LoadScene("GameInTime");
    }
    public void NewGameInHits()
    {
        SceneManager.LoadScene("GameInHits");
    }




    public void GoMenu2Records()
    {
        mainMenuScn.SetActive(false);
        recordsScn.SetActive(true);

        LocalDB._def_OpenedRecordsInMain = 1;

        GetRecords();
    }
    public void GoRecords2Menu()
    {
        mainMenuScn.SetActive(true);
        recordsScn.SetActive(false);

        LocalDB._def_OpenedRecordsInMain = 0;
    }

    public void GoToRurricGamesRu()
    {
        Application.OpenURL("https://rurricgames.ru/");
    }









    public void CheckOpenedRecords()
    {
        if ( LocalDB._def_OpenedRecordsInMain == 1 )
            GoMenu2Records();
        else
            GoRecords2Menu();
    }

    public void GetRecords()
    {
        int countTime = PlayerPrefs.GetInt("_rec_GameInTime2", -1);
        fieldRecordInTime2.text = ( countTime == -1 ) ? " - " : GameInTime.GameManager.TimestampToString(countTime);

        int countHits = PlayerPrefs.GetInt("_rec_GameInHits2", -1);
        fieldRecordInHits2.text = ( countHits == -1 ) ? " - " : countHits.ToString();


        countTime = PlayerPrefs.GetInt("_rec_GameInTime3", -1);
        fieldRecordInTime3.text = (countTime == -1) ? " - " : GameInTime.GameManager.TimestampToString(countTime);

        countHits = PlayerPrefs.GetInt("_rec_GameInHits3", -1);
        fieldRecordInHits3.text = (countHits == -1) ? " - " : countHits.ToString();

    }

    public void ResetRecords()
    {
        PlayerPrefs.SetInt("_rec_GameInTime2", -1);
        fieldRecordInTime2.text = " - ";

        PlayerPrefs.SetInt("_rec_GameInHits2", -1);
        fieldRecordInHits2.text = " - ";

        PlayerPrefs.SetInt("_rec_GameInTime3", -1);
        fieldRecordInTime3.text = " - ";

        PlayerPrefs.SetInt("_rec_GameInHits3", -1);
        fieldRecordInHits3.text = " - ";
    }

    public void ExitRGB()
    {
        Application.Quit();
    }
}
