using UnityEngine;
using System.Collections;
using System;

public class UIController : MonoBehaviour 
{
	public float m_pGameTime = 300.0f;
	public UISprite m_pMiaoBaiwei;
	public UISprite m_pMiaoshiwei;
	public UISprite m_pMiaogewei;
	public UISprite m_pMiaobiaozhi;
	public UIAtlas m_pNormalAtlas;
	public UIAtlas m_pWarnAtlas;
	public TweenScale m_pScale;
	private bool m_pHasChange = false;
	public PlayerController m_Player;
	public GameObject m_CongratulateJiemian;
	public GameObject m_FinishiJiemian;
	public GameObject m_OverJiemian;
	public GameObject m_CongratulateZitiObj;
	public GameObject m_FinishiZitiObj;
	public GameObject m_OverZitiObj;
	public GameObject m_JiluObj;
	public GameObject m_JindutiaoObj;
	public GameObject m_daojishiObj;
	public GameObject m_biaodituObj;
	private float m_CongratulateTimmer = 0.0f;
	private bool m_IsCongratulate = false;
	public CameraShake m_CameraShake;
	private bool m_HasShake = false;
	public bool m_IsGameOver = false;
	private int m_Score = 0;
	private int m_totalTime = 0;
	private int m_JiluRecord = 0;
	
	public UISprite m_ScoreFenGewei;
	public UISprite m_ScoreMiaoShiwei;
	public UISprite m_ScoreMiaoGewei;
	public UISprite m_RecordFenGewei;
	public UISprite m_RecordMiaoShiwei;
	public UISprite m_RecordMiaoGewei;

	public float Distance = 6400;
	public UISprite m_JinduTiao;
	public Transform m_ChuanTuBiao;

	public UITexture m_BeginDaojishi;
	public Texture[] m_BeginDaojishiTexture;

	//youmentishi
	public UITexture m_YoumenTishi;
	public Texture[] m_YoumenTishiTexture;
	private float m_YoumenTimmer = 0.0f;
	public AudioSource m_GameOverAudio;
	public AudioSource m_FinishiAudio;
	public AudioSource m_NewRecordAudio;
	public AudioSource m_NewRecordHitAudio;
	private bool m_HasTishi = false;
	public AudioSource m_BeginDaojishiAudio;
	private bool m_HasPlay = false;

	void Start () 
	{
        m_pGameTime = 120.0f;   //gzknu
        m_pMiaoshiwei.spriteName = "2"; //gzknu

        chile = 0;
		m_pScale.enabled = false;
		m_totalTime = (int)m_pGameTime;
		XkGameCtrl.IsLoadingLevel = false;

		//InputEventCtrl.GetInstance().ClickPlayerYouMenBtEvent += ClickPlayerYouMenBtEvent;
	}

	bool IsCloseYouMenTiShi;
	/*void ClickPlayerYouMenBtEvent(ButtonState val)
	{
		if (val == ButtonState.UP) {
			return;
		}
		IsCloseYouMenTiShi = true;
	}*/

	void Update ()
	{
		if(PlayerController.GetInstance().timmerstar < 5.0f)
		{
            //Debug.Log("11111111111111111111111111111111111111111111");

            //gzkun void CloseAllQiNang()
            if (!SetPanel.IsOpenSetPanel)
            {                
                pcvr.m_IsOpneForwardQinang = false;
                pcvr.m_IsOpneBehindQinang = false;
                pcvr.m_IsOpneLeftQinang = false;
                pcvr.m_IsOpneRightQinang = false;
            }

            UpdateBeginDaojishi();
		}
		else
		{
			if (pcvr.mGetPower > 0f) {
				IsCloseYouMenTiShi = true;
			}

			if(m_BeginDaojishi.enabled)
			{
				m_BeginDaojishi.enabled = false;
				m_BeginDaojishiAudio.Stop();
			}
			if(!IsCloseYouMenTiShi && !m_HasTishi)
			{
				m_YoumenTishi.enabled = true;
				UpdateYoumenTishi();
			}
			else
			{
				m_HasTishi = true;
				m_YoumenTishi.enabled = false;
				m_YoumenTimmer = 0.0f;
			}
			if(m_pGameTime>=0.0f && !m_Player.m_IsFinished)
			{
				UpdateJinduTiao();
				UpdateGameTime();
			}
			else
			{
				if(m_pGameTime<=0.0f)
				{
					m_IsGameOver = true;
					TouBiInfoCtrl.IsCloseQiNang = true;
				}
				m_pScale.enabled = false;
			}
			if(m_Player.m_timmerFinished>2.0f && !m_IsCongratulate)
			{
				if(m_Player.m_IsFinished)
				{
					m_Score =  (int)(m_totalTime + chile * addChiLe - m_pGameTime);
					m_JiluRecord = ReadGameInfo.GetInstance().ReadGameRecord();
					if(m_JiluRecord == 0 || m_Score<m_JiluRecord)
					{
						if(!m_NewRecordAudio.isPlaying)
							m_NewRecordAudio.Play();
						m_CongratulateJiemian.SetActive(true);
						ReadGameInfo.GetInstance().WriteGameRecord(m_Score);
					}
					else
					{
						if(!m_FinishiAudio.isPlaying)
							m_FinishiAudio.Play();
						m_FinishiJiemian.SetActive(true);
					}
					m_JiluObj.SetActive(true);
					UpdateMyScore();
					UpdateRecord();
				}
				else
				{
					if(!m_GameOverAudio.isPlaying)
						m_GameOverAudio.Play();
					m_OverJiemian.SetActive(true);
				}
				m_IsCongratulate = true;
				m_JindutiaoObj.SetActive(false);
				m_daojishiObj.SetActive(false);
				m_biaodituObj.SetActive(false);
			}
			if(m_IsCongratulate)
			{
				m_CongratulateTimmer+=Time.deltaTime;
			}
			if(m_CongratulateTimmer > 1.0f)
			{
				if(m_Player.m_IsFinished)
				{
					if(m_Score < m_JiluRecord || m_JiluRecord == 0)
					{
						if(!m_NewRecordHitAudio.isPlaying && !m_HasPlay)
						{
							m_HasPlay = true;
							m_NewRecordHitAudio.Play();
						}
							
						m_CongratulateZitiObj.SetActive(true);
					}
					else
					{
						m_FinishiZitiObj.SetActive(true);
					}
				}
				else
				{
					m_OverZitiObj.SetActive(true);
				}
			}
			if(m_Player.m_IsFinished && m_CongratulateTimmer>1.2f && !m_HasShake)
			{
				m_HasShake = true;
				m_CameraShake.setCameraShakeImpulseValue();
			}
			if(m_CongratulateTimmer>5.0f)
			{
				if (pcvr.bIsHardWare) {
					MyCOMDevice.GetInstance().ForceRestartComPort();
				}
				XkGameCtrl.IsLoadingLevel = true;
				LoadMovieLevel();
			}
		}
	}

	bool IsLoadMovie;
	void LoadMovieLevel()
	{
		if (IsLoadMovie) {
			return;		
		}
		IsLoadMovie = true;

		StartCoroutine(CheckUnloadUnusedAssets());
	}

	IEnumerator CheckUnloadUnusedAssets()
	{
		bool isLoop = true;
		GC.Collect();
		AsyncOperation asyncVal = Resources.UnloadUnusedAssets();
		float timeLast = Time.realtimeSinceStartup;

		do {
			yield return new WaitForSeconds(0.5f);
			if (Time.realtimeSinceStartup - timeLast > 5f) {
				isLoop = false;
				XkGameCtrl.IsLoadingLevel = true;
				//Debug.Log("CheckUnloadUnusedAssets -> loading movie level, asyncVal.isDone "+asyncVal.isDone);
				Application.LoadLevel(0);
				yield break;
			}

			if (!asyncVal.isDone) {
				yield return new WaitForSeconds(0.5f);
			}
			else {
				isLoop = false;
				XkGameCtrl.IsLoadingLevel = true;
				Application.LoadLevel(0);
				yield break;
			}
		} while (isLoop);
	}

	void UpdateGameTime()
	{
		m_pGameTime -= Time.deltaTime;
		int TimeMiaoBaiwei = (int)(m_pGameTime / 100);
		int TimeMiaoshiwei = (int)((m_pGameTime - TimeMiaoBaiwei*100)/10);
		int TimeMiaogewei = (int)(m_pGameTime - TimeMiaoBaiwei*100 - TimeMiaoshiwei*10);
		if(m_pGameTime <= 11.0f && !m_pHasChange)
		{
			m_pScale.enabled = true;
			m_pMiaoBaiwei.atlas = m_pWarnAtlas;
			m_pMiaoshiwei.atlas = m_pWarnAtlas;
			m_pMiaogewei.atlas = m_pWarnAtlas;
			m_pMiaobiaozhi.atlas = m_pWarnAtlas;
			m_pHasChange = true;
		}
		m_pMiaoBaiwei.spriteName = TimeMiaoBaiwei.ToString();
		m_pMiaoshiwei.spriteName = TimeMiaoshiwei.ToString();
		m_pMiaogewei.spriteName = TimeMiaogewei.ToString();
	}


	void UpdateMyScore()
	{
		int fen = m_Score/60;
		if(fen > 0)
		{
			m_ScoreFenGewei.spriteName = fen.ToString();
		}
		else
		{
			m_ScoreFenGewei.spriteName = "0";
		}
		int miao = m_Score - fen*60;
		int miaoshiwei = miao/10;
		int miaogewei = miao - miaoshiwei*10;
		m_ScoreMiaoShiwei.spriteName = miaoshiwei.ToString();
		m_ScoreMiaoGewei.spriteName = miaogewei.ToString();
	}


	void UpdateRecord()
	{
		int fen = m_JiluRecord/60;
		if(fen > 0)
		{
			m_RecordFenGewei.spriteName = fen.ToString();
		}
		else
		{
			m_RecordFenGewei.spriteName = "0";
		}
		int miao = m_JiluRecord - fen*60;
		int miaoshiwei = miao/10;
		int miaogewei = miao - miaoshiwei*10;
		m_RecordMiaoShiwei.spriteName = miaoshiwei.ToString();
		m_RecordMiaoGewei.spriteName = miaogewei.ToString();
	}

	void UpdateJinduTiao()
	{
		m_JinduTiao.fillAmount = (m_Player.m_distance)/Distance;
		if(m_JinduTiao.fillAmount > 1.0f)
		{
			m_JinduTiao.fillAmount = 1.0f;
		}
		m_ChuanTuBiao.localPosition = new Vector3(m_JinduTiao.fillAmount *(355+375.0f)-375.0f ,-25.0f,0.0f);
	}

	void UpdateBeginDaojishi()
	{
		if(!m_BeginDaojishi.enabled)
		{
			m_BeginDaojishi.enabled = true;
		}
		if (!m_BeginDaojishiAudio.isPlaying)
				m_BeginDaojishiAudio.Play ();
		int index = (int)(6.0f - PlayerController.GetInstance().timmerstar);
		if(index>=6)
		{
			index = 5;
		}
		m_BeginDaojishi.mainTexture = m_BeginDaojishiTexture[index-1];
	}

	void UpdateYoumenTishi()
	{
		m_YoumenTimmer+=Time.deltaTime;
		if(m_YoumenTimmer<0.3f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[0];
		}
		else if(m_YoumenTimmer>=0.3f &&m_YoumenTimmer<0.6f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[1];
		}
		else if(m_YoumenTimmer>=0.6f &&m_YoumenTimmer<0.9f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[2];
		}
		else if(m_YoumenTimmer>=0.9f &&m_YoumenTimmer<1.2f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[3];
		}
		else
		{
			m_YoumenTimmer = 0.0f;
		}
	}
	public GameObject m_JiashiTexture;
	public TweenScale m_Scale;
	public TweenPosition m_Position;
	private int chile = 0;
	private float addChiLe = 5.0f;
	public void ResetJiashi()
	{
		m_JiashiTexture.transform.localPosition = new Vector3 (0.0f, 0.0f, 500.0f);
		m_JiashiTexture.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
		if (!m_IsGameOver) {
			m_pGameTime += addChiLe;
			chile ++;
		}
		//Debug.Log ("chileeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee " +chile);
		m_Scale.ResetToBeginning ();
		m_Position.ResetToBeginning ();
		m_Scale.enabled = true;
		m_Position.enabled = true;
		m_JiashiTexture.SetActive (false);
	}
}
