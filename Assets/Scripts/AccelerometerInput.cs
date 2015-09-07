using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AccelerometerInput : MonoBehaviour {
	public float speed;
	public Text countText;
	public Text gameOverText;
	public Text resultText;
	public GameObject panel;
	public GameObject panelName;
	public GameObject panelTable;
	public GameObject panelOptions;
	public InputField inputFieldName;
	public GameObject[] myObjects;
	public Text[]scoreText;
	public Text[]nameText;
	public AudioSource audioMoney;
	public GameObject Pocket;
	public GameObject Finish;
	public GameObject Walls;
	public GameObject PocketStop;
	public GameObject PocketPlay;
	public Button Mute;
	public Color normalColor;
	public Color highlightedColor;

	private AudioSource audioWalls;
	private AudioSource audioPocket;
	private AudioSource audioWin;
	private bool gameOver;
	private Rigidbody rb;
	private int count;
	private bool nameComplete;
	private bool isPlayStop;
	private string highscorePos = "saved_sM";
	private string highscoreName = "saved_nM";
	private string muteCondition = "mute_cond";
	private int num;
	private ArrayList masScore;
	private ArrayList masName;
	private int tempScore;
	private string tempName;
    private Button b;
	private ColorBlock colorNotChecked;
	private ColorBlock colorChecked;
	private float originZ;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		originZ = -Input.acceleration.z;
		count = 0;
		inputFieldName.text = "";
		gameOver = false;
		gameOverText.text = "";
		resultText.text = "";
		panel = GameObject.Find ("PanelResult");
	    panelName = GameObject.Find ("PanelName");
		panelTable = GameObject.Find ("PanelTable");
		panelOptions = GameObject.Find ("PanelOptions");
		audioMoney = GetComponent<AudioSource>();
		audioPocket = Pocket.GetComponent<AudioSource>();
		audioWin = Finish.GetComponent<AudioSource>();
		audioWalls = Walls.GetComponent<AudioSource>();
		b = Mute.GetComponent<Button>(); 
		colorNotChecked = b.colors;
		colorChecked = b.colors;
		colorNotChecked.normalColor = normalColor;
		colorNotChecked.highlightedColor = normalColor;
		colorChecked.normalColor = highlightedColor;
		colorChecked.highlightedColor = highlightedColor;
		SetMuteCondition (GetBool (muteCondition));
		SetPanelsAcrive (false,false,false,false);
		SetCountText ();


		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		num = 0;
		masScore = new  ArrayList();
		masName = new ArrayList();
		for (int i =0; i <5;i++) {
			masScore.Add(0);
			masName.Add("");
		}

		for(int i =0; i<myObjects.Length; i++)
		{
			float offsetX = Random.Range (-10.0F, 10.0F);
			float offsetZ = Random.Range (-5.0F, 5.0F);

			if ((Mathf.Abs (offsetX + myObjects[i].transform.position.x)) > 12F) {
				offsetX = 0;
			}

			if ((Mathf.Abs (offsetZ + myObjects[i].transform.position.z)) > 7F) {
				offsetZ = 0;
			}
			Vector3 tmp = new Vector3 (offsetX, 0, offsetZ);
			myObjects[i].transform.position+=tmp;

		}

	}

	void Update(){
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) 
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.GetTouch(0).position );
			RaycastHit hit;
			
			if ( Physics.Raycast(ray, out hit)&&( (hit.transform.gameObject.name == "PocketPlay")||(hit.transform.gameObject.name == "PocketStop")))
			{
				if (PocketStop.activeSelf){
					PocketStop.SetActive(false);
					PocketPlay.SetActive(true);
					if (!isPlayStop){
				    gameOver = true;  
					}
					SetPanelsAcrive(false, false, false, true);

				}
				else{
					PocketStop.SetActive(true);
					PocketPlay.SetActive(false);
					if (!isPlayStop){
						gameOver = false;	
					}
					SetPanelsAcrive(false, false, false, false);
				
				}

			}
		}
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit(); 
			
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 movement;
		if (!gameOver) {
			float currentZ = -Input.acceleration.z;
			if (originZ > currentZ){
				movement = new Vector3 (Input.acceleration.x, 0, currentZ - originZ);
			
			}else{
				movement = new Vector3 (Input.acceleration.x, 0, currentZ);
			}
			rb.AddForce (movement * speed);
		} else {
			rb.velocity = Vector3.zero;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick up")) {
			other.gameObject.SetActive(false);
			count = count + 100;
			audioMoney.Play();
			SetCountText();
		}

		if (other.gameObject.CompareTag ("Pocket")) {
			if (!isPlayStop){
			audioPocket.Play();
			gameOver = true;
			string s = "You lose!";
			isPlayStop = true;
			setGameOverResult(s);
			}
		}

		if (other.gameObject.CompareTag ("Finish")) {
			if (!isPlayStop){
			GameOver("You win!");
			isPlayStop =true;
			audioWin.Play();
			}
		}

	}

	void SetCountText(){
		countText.text = "Count: " + count.ToString();
	}


	public void GameOver (string s)
	{   
		gameOver = true;
		for (int i =0; i <5;i++) {
			masScore[i] = PlayerPrefs.GetInt (highscorePos + i,0);
			masName[i]= PlayerPrefs.GetString (highscoreName + i,"");
		}


		for(int i=0; i <5;i++ ){
			if (count >(int) masScore[i]){
				SetPanelsAcrive(true, false, false, false);
				num =i;
				nameComplete = true;
				break;
			}
			else{
				num = 100;
			}
			
		}

		if (!nameComplete) {
			setGameOverResult(s);
		} 
	}


	public void setGameOverResult(string s){
		SetPanelsAcrive(false, true, false, false);
		gameOverText.text = s;
		resultText.text = "Result score: " + count;
	}
	public void  OnResultCLick(){
		Application.LoadLevel (Application.loadedLevel);
	}

	public void  OnExitCLick(){
		Application.Quit();
	}
	
	public void  OnMuteCLick(){
		SetMuteCondition (!audioWalls.mute);
	}
	
	public void  OnEnterNameCLick(){
		if (num<5){
			for(int i=4; i >=num+1;i-- ){
				masScore[i] = (int) masScore[i-1];
				masName[i] = (string) masName[i-1];
			}
		}
		if (num !=100) {
			masScore[num] = count;
			masName[num] = inputFieldName.text;

			for (int i = 0; i < 5; i++) {

				PlayerPrefs.SetInt(highscorePos + i, (int)masScore[i]);
				PlayerPrefs.SetString(highscoreName + i, (string)masName[i]);
			}
			
		}
		for (int i =0; i<scoreText.Length; i++){
			scoreText[i].text = (PlayerPrefs.GetInt (highscorePos + i,0)).ToString();
			nameText[i].text = PlayerPrefs.GetString (highscoreName + i,"");
		}
		SetPanelsAcrive(false, false, true, false);
	}
		
	void SetPanelsAcrive (bool pN, bool p, bool pT, bool pO){
		panelName.SetActive (pN);
		panel.SetActive (p);
		panelTable.SetActive (pT);
		panelOptions.SetActive (pO);
	}

	void SetMuteCondition(bool isMute){
		audioWin.mute = isMute;
		audioWalls.mute = isMute;
		audioPocket.mute = isMute;
		audioMoney.mute = isMute;
		
		SetBool (muteCondition, isMute);
		if (isMute) {
			Mute.colors = colorChecked;
		}
		else{
			Mute.colors = colorNotChecked;
		}
	}

	public static void SetBool(string key, bool state)
	{
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}
	
	public static bool GetBool(string key)
	{
		int value = PlayerPrefs.GetInt(key);
		
		if (value == 1)
		{
			return true;
		}
		
		else
		{
			return false;
		}
	}
}
