using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Utilities {

	public static GameManager GM;
	
	public AudioSource SwooshSound;
	
	public List<GameObject> CharPrefabs = new List<GameObject>();
	
	int NumPlayers;
	List<int> PlayerCharacters = new List<int>();
	
    [HideInInspector]
	public List<GameObject> PlayerRefs = new List<GameObject>();

    public GameObject TargetGroup;

    public GameObject Canvas;

    private bool isGameOver = false;
    private bool isGamePaused = false;

	void Awake(){
		if(GM == null){
			GM = this;
		}
		if(GM != this){
			Destroy(GM);
		}

        StartCoroutine(FadeOut(Canvas.transform.Find("BlackScreen").gameObject, 2f, 1f));

    }
	
	// Use this for initialization
	void Start () {
		
		if(GameObject.Find("CharacterSelectManager") != null){
			CharacterSelectManager csm = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
			PlayerCharacters = csm.SelectedCharacters;
			for(int i = 0; i < PlayerCharacters.Count; i++){
				if(PlayerCharacters[i] != -1){
					NumPlayers++;
				}
			}
			
			InstantiatePrefabs();
			SetCamera();
			SetUI();
			
			
			GameObject.Destroy(csm.gameObject);
		}
		else{
			print("deu merda");
		}		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick3Button7) || Input.GetKeyDown(KeyCode.Joystick4Button7) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
	}
	
	void InstantiatePrefabs(){
		
		for (int i = 0 ; i < NumPlayers; i++){
			GameObject obj = Instantiate(CharPrefabs[PlayerCharacters[i]], transform.GetChild(0).position, transform.GetChild(0).rotation);
			PlayerRefs.Add(obj);
			Destroy(transform.GetChild(i).gameObject);
			SetController(obj, i+1);
		}
		
		for(int i = NumPlayers; i < 4; i++)
			Destroy(transform.GetChild(i).gameObject);
	}
	
	void SetCamera()
	{
		for(int i = 0; i < NumPlayers; i++)
        {
            TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[i].target = PlayerRefs[i].transform;
            TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[i].weight = 1f;
            TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[i].radius = 0.5f;
        }
	}
	
	void SetController(GameObject obj, int index)
	{
		string str = "Joystick"+(index);
		obj.GetComponent<Movement>().Initiate(str); 		
	}
	
	void SetUI()
	{
		for(int i = 0; i < NumPlayers; i++)
        {
			GameObject ui = Canvas.transform.GetChild(i).gameObject;
			
            ui.SetActive(true);
            
        }
	}

    void GameOver()
    {
		print("game over");
		//fade in black screen
        Canvas.transform.Find("GameOverPanel").gameObject.SetActive(true);
		Canvas.transform.Find("GameOverPanel").GetComponent<PauseMenuBehaviour>().SelectFirstButton();
        isGameOver = true;
    }


	/**
	 * Função chamada quando os jogadores chegarem no Floofy
	 */
	public void Victory(){
		//SETAR PAINEL DE FIM DE JOGO -------------------------	
		isGameOver = true;
		print ("Huzzah!");
	}



    void PauseGame()
    {
        Canvas.transform.Find("PausePanel").gameObject.SetActive(true);
		Canvas.transform.Find("PausePanel").GetComponent<PauseMenuBehaviour>().SelectFirstButton();
        isGamePaused = true;
		Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Canvas.transform.Find("PausePanel").gameObject.SetActive(false);
        isGamePaused = false;
		Time.timeScale = 1;
    }
	
	public void BackToMainMenu(){
		Time.timeScale = 1;
		SwooshSound.Play();
        StartCoroutine(FadeIn(Canvas.transform.Find("BlackScreen").gameObject, 2f, 1f));
		
		if(GameObject.Find("Music") != null)
            GameObject.Find("Music").name = "RealMusic";
		StartCoroutine(BackToMenuDelay(2f));
	}
	
	IEnumerator BackToMenuDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }
}