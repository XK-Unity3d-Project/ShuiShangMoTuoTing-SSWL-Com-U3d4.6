using UnityEngine;
using System.Collections;
using System;

public class Loading : MonoBehaviour
{
	private string CoinNumSet = "1";
	private string InsertCoinNum = "";

	public UITexture m_BeginTex;
	public Texture   m_LoadingTex;
	public UITexture m_InsertTex;
	public UISprite CoinNumSetTex;
	public UISprite m_InsertNumS;
	public UISprite m_InsertNumG;
	private int m_InserNum = 0;
	private bool m_IsBeginOk = false;

	private float m_PressTimmer = 0.0f;
	private float m_InserTimmer = 0.0f;
	private bool m_IsStartGame =false;
	public AudioSource m_TbSource;
	public AudioSource m_BeginSource;
	public GameObject m_Loading;
	public GameObject m_Tishi;
	private string GameMode = "";

	public UITexture m_pTishiTexture;
	public Texture[] m_pTexture;

	public MovieTexture m_MovieTex;
	public UITexture m_FreeTexture;
	public GameObject m_ToubiObj;

	private bool timmerstar = false;
	private float timmerforstar = 0.0f;
	public static bool m_HasBegin = false;
	public bool IsLuPingTest;
	void Start ()
	{
		if (IsLuPingTest) {
			gameObject.SetActive(false);
		}
		//m_MovieTex.loop = true;
		//m_MovieTex.Play();
		m_HasBegin = false;
		GameMode = ReadGameInfo.GetInstance ().ReadGameStarMode();
		if(GameMode == "oper")
		{
			m_FreeTexture.enabled = false;
			CoinNumSet = ReadGameInfo.GetInstance ().ReadStarCoinNumSet();
			InsertCoinNum = ReadGameInfo.GetInstance ().ReadInsertCoinNum();
			CoinNumSetTex.spriteName = CoinNumSet;
			m_InserNum = Convert.ToInt32(InsertCoinNum);
			UpdateInsertCoin();
			UpdateTex();
		}
		else
		{
			m_ToubiObj.SetActive(false);
			m_IsBeginOk = true;
			m_FreeTexture.enabled = true;
		}
		//Invoke("OnClickBeginBt", UnityEngine.Random.Range(1f, 5f)); //test
		m_Loading.SetActive(false);
		pcvr.ShaCheBtLight = StartLightState.Mie;

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
	}
	void Update ()
	{
		if (!m_IsStartGame) {
			UpdateTex();
		}

		if (pcvr.bIsHardWare) {
			if (GlobalData.CoinCur != m_InserNum && GameMode == "oper") {
				m_InserNum = GlobalData.CoinCur - 1;
				OnClickInsertBt();
			}
		}
		else {
			if(Input.GetKeyDown(KeyCode.T) && GameMode == "oper")
			{
				OnClickInsertBt();
			}
		}
		OnLoadingClicked();
	}

	void ClickStartBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		OnClickBeginBt();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}

		if (Loading.m_HasBegin) {
			return;
		}
		
		XkGameCtrl.IsLoadingLevel = true;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		Application.LoadLevel(2);
	}

	void UpdateInsertCoin()
	{
		int n = 1;
		int num = m_InserNum;
		int temp = num;
		while(num > 9)
		{
			num /= 10;
			n++;
		}
		if(n > 2)
		{
			m_InsertNumS.spriteName = "9";
			m_InsertNumG.spriteName = "9";
		}
		else if(n==2)
		{
			int shiwei = (int)(temp/10);
			int gewei = (int)(temp-shiwei*10);
			m_InsertNumS.spriteName = shiwei.ToString();
			m_InsertNumG.spriteName = gewei.ToString();
		}
		else if(n == 1)
		{
			m_InsertNumS.spriteName = "0";
			m_InsertNumG.spriteName = temp.ToString();
		}
	}

	void UpdateTex()
	{
		if(GameMode == "FREE" || m_InserNum >= Convert.ToInt32(CoinNumSet))
		{
			m_InserTimmer = 0.0f;
			m_IsBeginOk = true;
			m_InsertTex.enabled = false;
			m_BeginTex.enabled =true;
			m_pTishiTexture.enabled = true;
			m_PressTimmer+=(Time.deltaTime / Time.timeScale);
			if(m_PressTimmer >= 0.0f && m_PressTimmer <= 0.5f)
			{
				m_BeginTex.enabled =true;
				m_pTishiTexture.mainTexture = m_pTexture[0];
			}
			else if(m_PressTimmer > 0.5f && m_PressTimmer <= 1.0f)
			{
				m_BeginTex.enabled =false;
				m_pTishiTexture.mainTexture = m_pTexture[1];
			}
			else
			{
				m_PressTimmer = 0.0f;
			}
			pcvr.StartBtLight = StartLightState.Shan;
		}
		else
		{
			pcvr.StartBtLight = StartLightState.Mie;
			m_InserTimmer+=(Time.deltaTime / Time.timeScale);
			m_IsBeginOk = false;
			m_InsertTex.enabled = true;
			m_BeginTex.enabled =false;
			m_pTishiTexture.enabled = false;
			m_PressTimmer = 0.0f;
			if(m_InserTimmer >= 0.0f && m_InserTimmer <= 0.4f)
			{
				m_InsertTex.enabled = true;
			}
			else if(m_InserTimmer > 0.4f && m_InserTimmer <= 0.8f)
			{
				m_InsertTex.enabled = false;
			}
			else
			{
				m_InserTimmer = 0.0f;
			}
		}
	}
	IEnumerator loadScene(int num)   
	{
		XkGameCtrl.IsLoadingLevel = true;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		AsyncOperation async = Application.LoadLevelAsync(num);   
		yield return async;		
	}

	void OnClickInsertBt()
	{
			m_TbSource.Play();
			m_InserNum++;
			ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
			UpdateInsertCoin();
	}
	void OnClickBeginBt()
	{
		if (PlayerControllerForMoiew.IsLoadMovieLevel) {
			return;
		}

		if(m_IsBeginOk && !m_HasBegin)
		{
			m_BeginSource.Play();
			m_IsStartGame = true;
			if(GameMode == "oper")
			{
				m_InserNum -= Convert.ToInt32(CoinNumSet);
				UpdateInsertCoin();
				ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());

				if (pcvr.bIsHardWare) {
					pcvr.GetInstance().SubPlayerCoin(Convert.ToInt32(CoinNumSet));
				}
			}
			m_Tishi.SetActive(false);
			m_Loading.SetActive(true);
			timmerstar = true;
			m_HasBegin = true;
		}
	}
	void OnLoadingClicked()
	{
		if(timmerstar)
		{
			timmerforstar+=Time.deltaTime;
			if(timmerforstar>1.5f)
			{
				StartCoroutine (loadScene(1));
				timmerstar = false;
			}
		}
	}
}
