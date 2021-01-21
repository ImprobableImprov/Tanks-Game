using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
   // public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;
    public GameObject m_SmallTurretPrefab;
    public GameObject m_MediumTurretPrefab;
    public GameObject m_LargeTurretPrefab;
    public TankManager[] m_Tanks;
    public TankManager[] m_SmallTurrets;
    public TankManager[] m_MediumTurrets;
    public TankManager[] m_LargeTurrets;


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SpawnAllTurrets();

        //StartCoroutine(GameLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown("r") || isAlive())
        {
            SceneManager.LoadScene("Tanks");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SpawnAllTurrets()
    {
        for (int i = 0; i < m_SmallTurrets.Length; i++)
        {
            m_SmallTurrets[i].m_Instance =
                Instantiate(m_SmallTurretPrefab, m_SmallTurrets[i].m_SpawnPoint.position, m_SmallTurrets[i].m_SpawnPoint.rotation) as GameObject;
            m_SmallTurrets[i].Setup();
        }
        for (int i = 0; i < m_MediumTurrets.Length; i++)
        {
            m_MediumTurrets[i].m_Instance =
                Instantiate(m_MediumTurretPrefab, m_MediumTurrets[i].m_SpawnPoint.position, m_MediumTurrets[i].m_SpawnPoint.rotation) as GameObject;
            m_MediumTurrets[i].Setup();
        }
        for (int i = 0; i< m_LargeTurrets.Length; i++)
        {
            m_LargeTurrets[i].m_Instance =
                Instantiate(m_LargeTurretPrefab, m_LargeTurrets[i].m_SpawnPoint.position, m_LargeTurrets[i].m_SpawnPoint.rotation) as GameObject;
            m_LargeTurrets[i].Setup();
        }
    }

    /*
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }*/

    
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundPlaying());

       if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop()); // ... 'yield return' so it doesn't 'loop' back
        }
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        //m_MessageText.text = string.Empty;

        while (!isAlive() || Input.GetKeyDown(KeyCode.R))
        {
            yield return null;
        }

    }


    private bool isAlive()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft == 0;
    }

    /*
    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }
    */

    /*
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }
    */

    /*
    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }
    */

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}