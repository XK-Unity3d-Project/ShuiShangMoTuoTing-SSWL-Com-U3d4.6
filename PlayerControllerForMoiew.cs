using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerControllerForMoiew : MonoBehaviour
{
//	private bool m_IsFinished = false;
	public CameraShake m_CameraShake;
	public AudioSource m_HitStone;
	public GameObject m_HitEffectObj;
	public AudioSource m_HitWater;
	public AudioSource m_YinqingAudio;
	public AudioSource m_ShuihuaAudio;
	public AudioSource m_BeijingAudio;
	public AudioSource m_HuanjingSenlin;
	public AudioSource m_HuanjingShuiliu;
	public AudioSource m_FeibanAudio;
	public GameObject m_FeibanEffectObj;

	public GameObject m_TeXiao0Audio;
	public GameObject m_TeXiao1Audio;
	public GameObject m_TeXiao2Audio;
	public GameObject m_TeXiao3Audio;
	public GameObject m_TeXiao4Audio;

	private bool m_Isdapubu = false;
	public GameObject[] m_Texture;

	//zhiwujiansu
	public Animator m_PlayerAnimator;
//	private bool m_IsJiasu = false;
	public float m_JiasuTimeSet = 3.0f;
//	private float m_JiasuTimmer = 0.0f;
	public GameObject m_JiasuPartical;
	public AudioSource m_JiasuAudio;
	public AudioSource m_EatJiasuAudio;
	public float m_JiasuTopSpeed = 0.0f;
	public GameObject m_JiashiGameObject;

	public GameObject m_JiashiPartical;
	public AudioSource m_JiashiAudio;
	public AudioSource m_EatJiashiAudio;
	public static PlayerControllerForMoiew Instance = null;

	public GameObject m_HitWaterParticle;
	public float m_BaozhaForward = 0.0f;
	public float m_BaozhaUp = 0.0f;
	public GameObject[] m_particalEffect;
	public CameraCtForMoivew m_Ctrlcamera;
	public ORNavigation m_otherCamerCtrl;
	public GameObject m_niao;
	public ColorCorrectionCurves m_HuiEffect;
	/**
	 * 主角引擎音效.
	 */
	public AudioSource PlayerYinQingAd;

	bool IsThreeScreen = false;
	public iTweenEvent ITweenEventCom;
	float HuiEffectSaturation;
	void Awake()
	{
		AudioListener.volume = (float)ReadGameInfo.GetInstance().ReadGameAudioVolume() / 10f;
	}

	void Start()
	{
		IsLoadMovieLevel = false;
		HuiEffectSaturation = m_HuiEffect.saturation;
		pcvr.IsSlowLoopCom = true;
		pcvr.CloseFangXiangPanPower();
		Screen.showCursor = false;
		if (IsThreeScreen) {
			Screen.SetResolution(1360*3, 768, true);
			//Screen.SetResolution((int)(1360*0.5), (int)(768*0.5), false);
		}
		else {
			Screen.SetResolution(1360, 768, true);
		}

		m_BeijingAudio.Play();
		Instance = this;
		XkGameCtrl.IsLoadingLevel = false;
		PlayerYinQingAd.Play();
		m_HuanjingSenlin.Play();
		m_HuanjingShuiliu.Play();

//		float timeValRand = UnityEngine.Random.Range(3f, 10f);
//		Invoke("DelayPcvrJiaMiJiaoYan", timeValRand);
	}
	
	void DelayPcvrJiaMiJiaoYan()
	{
		//Debug.Log("DelayPcvrJiaMiJiaoYan...");
		pcvr.GetInstance().StartJiaoYanIO();
	}

	public static PlayerControllerForMoiew GetInstance()
	{
		return Instance;
	}

	public Vector3 StartPos;
	public Vector3 NextPos;
	void DelayOpenPlayerCamera()
	{
		m_CameraShake.camera.enabled = true;
	}

	void ReplayStartCartoon()
	{
		if (m_BeijingAudio.isPlaying) {
			return;
		}

		m_HasPlay = false;
		m_CameraShake.camera.enabled = false;
		transform.forward = Vector3.Normalize(NextPos - StartPos);
		transform.position = StartPos;
		m_EndTextureTimmer = 0f;
		m_LiangTimmer = 0f;
		if (!m_BeijingAudio.isPlaying) {
			m_BeijingAudio.Play();
		}
		ShowAllDaoJuListObj();
		ResetEndTextureScale();

		m_HuiEffect.saturation = HuiEffectSaturation;
		m_EndTexture.SetActive(false);
		ITweenEventCom.Start();
		Invoke("DelayOpenPlayerCamera", 0.5f);
	}

	void Update () 
	{
		UpdateEndTexture ();
		if (Time.frameCount % 200 == 0) {
			GC.Collect();
		}
	}

	void FixedUpdate()
	{
		m_Wuya.speed = 1.0f / Time.timeScale;
	}

//	private bool m_hasplay = false;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "finish")
		{
			m_PlayerAnimator.speed = 3.8f/Time.timeScale;
//			m_IsFinished = true;
			m_PlayerAnimator.SetBool("IsFinish",true);
			m_PlayerAnimator.SetBool("IsRoot",false);
		}
		if(other.tag == "zhangai")
		{
			m_PlayerAnimator.SetTrigger("IsZhuang");
			m_CameraShake.setCameraShakeImpulseValue();
			m_HitStone.Play();
			GameObject temp = (GameObject)Instantiate(m_HitEffectObj,transform.position,transform.rotation);
			Destroy(temp,2.0f);
		}
		if(other.tag == "texiao0")
		{
			m_PlayerAnimator.SetTrigger("IsQifei");
			m_FeibanAudio.Play();		
		//	m_TeXiao0Audio.Play();
			GameObject temp = (GameObject)Instantiate(m_TeXiao0Audio,transform.position,transform.rotation);
			Destroy(temp,3.0f);
		}
		if(other.tag == "texiao1")
		{
			m_PlayerAnimator.SetTrigger("IsDiaoluo");
			m_FeibanAudio.Play();
		//	m_TeXiao1Audio.Play();
			GameObject temp = (GameObject)Instantiate(m_TeXiao1Audio,transform.position,transform.rotation);
			Destroy(temp,3.0f);
		}
		if(other.tag == "texiao2")
		{
			//Debug.Log("IsZuantoufjkfjfkl");
			m_PlayerAnimator.SetTrigger("IsZuantou");
			if(!m_Isdapubu)
			{
				m_FeibanAudio.Play();
				GameObject temp = (GameObject)Instantiate(m_TeXiao2Audio,transform.position,transform.rotation);
				Destroy(temp,4.2f);
			}
			else
			{
				m_Isdapubu = false;
			}
		}
		if(other.tag == "texiao3")
		{
			m_PlayerAnimator.SetTrigger("IsTaitou");
			m_FeibanAudio.Play();
		//	m_TeXiao3Audio.Play();
			GameObject temp = (GameObject)Instantiate(m_TeXiao3Audio,transform.position,transform.rotation);
			Destroy(temp,3.0f);
		}
		if(other.tag == "texiao4")
		{
			m_PlayerAnimator.SetTrigger("IsDianshan");
			m_FeibanAudio.Play();
		//	m_TeXiao4Audio.Play();
			GameObject temp = (GameObject)Instantiate(m_TeXiao4Audio,transform.position,transform.rotation);
			Destroy(temp,4.2f);
		}
		if(other.tag == "texiao5")
		{
			m_Isdapubu = true;
			m_PlayerAnimator.SetTrigger("IsTaitou");
			m_FeibanAudio.Play();
			GameObject temp = (GameObject)Instantiate(m_FeibanEffectObj,transform.position,transform.rotation);
			Destroy(temp,2.0f);
		}
		if(other.tag == "texiaoyin")
		{
		//	m_Isdapubu = true;
		//	m_PlayerAnimator.SetTrigger("IsTaitou");
			m_FeibanAudio.Play();
		}
		if(other.tag == "left")
		{
			m_PlayerAnimator.SetBool("IsTurnleft",true);
			m_PlayerAnimator.SetBool("IsTurnRight",false);
			m_PlayerAnimator.SetBool("IsRoot",false);
			m_PlayerAnimator.speed = 1.0f/Time.timeScale;
			//Debug.Log("m_PlayerAnimator.speed" + m_PlayerAnimator.speed);
		}
		if(other.tag == "right")
		{
			m_PlayerAnimator.SetBool("IsTurnRight",true);
			m_PlayerAnimator.SetBool("IsTurnleft",false);
			m_PlayerAnimator.SetBool("IsRoot",false);
			m_PlayerAnimator.speed = 1.0f/Time.timeScale;
			//Debug.Log("m_PlayerAnimator.speed" + m_PlayerAnimator.speed);
		}
		if(other.tag == "root")
		{
			//m_PlayerAnimator.speed = 1.0f/Time.timeScale;
			m_PlayerAnimator.SetBool("IsDianshan1",false);
			m_PlayerAnimator.SetBool("IsRoot",true);
			m_PlayerAnimator.SetBool("IsTurnRight",false);
			m_PlayerAnimator.SetBool("IsTurnleft",false);
		}
		if(other.tag == "hitwater")
		{
			m_CameraShake.setCameraShakeImpulseValue();
			m_HitWater.Play();
			GameObject Tobject = (GameObject)Instantiate(m_HitWaterParticle,transform.position+transform.forward*m_BaozhaForward+Vector3.up*m_BaozhaUp,transform.rotation);
			Destroy(Tobject,0.5f);
		}
		if(other.tag == "hitwater0")
		{
		//	m_CameraShake.setCameraShakeImpulseValue();
			m_HitWater.Play();
			GameObject Tobject = (GameObject)Instantiate(m_HitWaterParticle,transform.position+transform.forward*m_BaozhaForward+Vector3.up*m_BaozhaUp,transform.rotation);
			Destroy(Tobject,0.5f);
		}
		if(other.tag == "dan1")
		{
			m_EatJiasuAudio.Play();
			m_JiasuAudio.Play();
			GameObject temp = (GameObject)Instantiate(m_JiasuPartical,other.transform.position+transform.forward*10.0f,other.transform.rotation);
			Destroy(temp,0.5f);
			//Destroy(other.gameObject);
			CheckDaoJuListObj(other.gameObject);
		}
		if(other.tag == "dan3")
		{
			m_EatJiasuAudio.Play();
			m_JiasuAudio.Play();
			GameObject temp = (GameObject)Instantiate(m_JiasuPartical,other.transform.position+transform.forward,other.transform.rotation);
			Destroy(temp,0.5f);
			//Destroy(other.gameObject);
			CheckDaoJuListObj(other.gameObject);
		}
		if(other.tag == "zhong")
		{
			GameObject temp = (GameObject)Instantiate(m_JiashiPartical,other.transform.position,other.transform.rotation);
			//Destroy(other.gameObject);
			CheckDaoJuListObj(other.gameObject);
			Destroy(temp,0.5f);
			m_EatJiashiAudio.Play();
			m_JiashiAudio.Play();
		}
		if(other.tag == "paizhao0")
		{
			m_Texture[0].SetActive(true);
		}
		if(other.tag == "paizhao1")
		{
			m_Texture[1].SetActive(true);
		}
		if(other.tag == "paizhao2")
		{
			m_Texture[2].SetActive(true);
		}
		if(other.tag == "paizhao3")
		{
			m_Texture[3].SetActive(true);
		}
		if(other.tag == "paizhao4")
		{
			m_Texture[4].SetActive(true);
		}
		if(other.tag == "dianshan1")
		{
			m_PlayerAnimator.SetTrigger("IsDianshan1");
			m_PlayerAnimator.SetBool("IsTurnRight",false);
			m_PlayerAnimator.SetBool("IsTurnleft",false);
			m_PlayerAnimator.SetBool("IsRoot",false);

			//m_PlayerAnimator.SetTrigger("IsQifei");
			m_FeibanAudio.Play();
		//	m_TeXiao1Audio.Play();
			GameObject temp = (GameObject)Instantiate(m_FeibanEffectObj,transform.position,transform.rotation);
			Destroy(temp,4.2f);

		}
		if(other.tag == "offshuihua")
		{
			m_particalEffect[0].SetActive(false);
			m_particalEffect[1].SetActive(false);
			m_particalEffect[2].SetActive(false);
			m_ShuihuaAudio.Stop();
		}
		if(other.tag == "openshuihua")
		{
			m_particalEffect[0].SetActive(true);
			m_particalEffect[1].SetActive(true);
			m_particalEffect[2].SetActive(true);
			m_ShuihuaAudio.Play();
		}
		if(other.tag == "opencamera")
		{
			m_otherCamerCtrl.enabled = false;
			m_Ctrlcamera.enabled = true;
		}
		if(other.tag == "showniao")
		{
			m_niao.SetActive(true);
			iTweenEvent iTweenEventCom = m_niao.GetComponent<iTweenEvent>();
			iTweenEventCom.Start();
		}
		if(other.tag == "hideniao")
		{
			m_niao.SetActive(false);
		}
		if(other.tag == "hui")
		{
			m_HuiEffect.saturation = 0.0f;
			m_EndTexture.SetActive(true);
		}
		if(other.tag == "jianyin")
		{
			ShowHidenCtrl.m_IsOpen = true;
		}
//		if(other.tag == "changecamera")
//		{
//			iTweenEvent.GetEvent(this.gameObject,"New Path1").Stop();
//			iTweenEvent.GetEvent(this.gameObject,"New Path2").Play();
//		}
	}

	List<GameObject> DaoJuListObj = new List<GameObject>();
	void CheckDaoJuListObj(GameObject obj)
	{
		if (!DaoJuListObj.Contains(obj)) {
			DaoJuListObj.Add(obj);
		}

		if (obj.activeSelf) {
			obj.SetActive(false);
		}
	}

	void ShowAllDaoJuListObj()
	{
		int max = DaoJuListObj.Count;
		for (int i = 0; i < max; i++) {
			DaoJuListObj[i].SetActive(true);
		}
	}

	public AudioSource m_TitleAudio;
	private bool m_HasPlay = false;
	void ResetEndTextureScale()
	{
		int max  = m_EndTextrues.Length;
		for (int i = 0; i < max; i++) {
			m_EndTextrues[i].transform.localScale = new Vector3(20f, 20f, 20f);
			m_EndTextrues[i].SetActive(false);
		}
	}

	void PlayTweenScale(GameObject obj)
	{
		TweenScale tweenScaleCom = obj.GetComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.ResetToBeginning();
		obj.SetActive(true);
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

	void UpdateEndTexture()
	{
		if(m_EndTexture.activeSelf)
		{
			m_PlayerAnimator.SetBool("IsFinish",false);
			m_PlayerAnimator.SetBool("IsRoot",true);
			m_EndTextureTimmer+=Time.deltaTime;
			if(!m_TitleAudio.isPlaying && !m_HasPlay)
			{
				m_TitleAudio.Play();
				m_HasPlay = true;
			}
			if(m_EndTextureTimmer > m_EndTextureTimmerSet && !m_EndTextrues[0].activeSelf)
			{
				PlayTweenScale(m_EndTextrues[0]);
			}
			if(m_EndTextureTimmer > 2*m_EndTextureTimmerSet && !m_EndTextrues[1].activeSelf)
			{
				PlayTweenScale(m_EndTextrues[1]);
			}
			if(m_EndTextureTimmer > 3*m_EndTextureTimmerSet && !m_EndTextrues[2].activeSelf)
			{
				PlayTweenScale(m_EndTextrues[2]);
			}
			if(m_EndTextureTimmer > 4*m_EndTextureTimmerSet && !m_EndTextrues[3].activeSelf)
			{
				PlayTweenScale(m_EndTextrues[3]);
			}
			if(m_EndTextureTimmer > 5*m_EndTextureTimmerSet && !m_EndTextrues[4].activeSelf)
			{
				PlayTweenScale(m_EndTextrues[4]);
				m_BeijingAudio.Stop();
			}
			m_LiangTimmer+=Time.deltaTime;
			if(m_LiangTimmer<m_LiangTimmerSet)
			{
				m_LiangTitle.SetActive(true);
			}
			else if(m_LiangTimmer>=m_LiangTimmerSet && m_LiangTimmer<=2*m_LiangTimmerSet)
			{
				m_LiangTitle.SetActive(false);
			}
			else
			{
				m_LiangTimmer = 0.0f;
			}
			if(m_EndTextureTimmer>6.5f)
			{
				if (Loading.m_HasBegin) {
					return;
				}
				StartCoroutine(CheckUnloadUnusedAssets());
			}
		}
	}

	IEnumerator CheckUnloadUnusedAssets()
	{
		bool isLoop = true;
		GC.Collect();
		AsyncOperation asyncVal = Resources.UnloadUnusedAssets();

		do {
			if (!asyncVal.isDone) {
				yield return new WaitForSeconds(0.5f);
			}
			else {
				ReplayStartCartoon();
				yield break;
			}
		} while (isLoop);
	}

	public static bool IsLoadMovieLevel = false;
	public GameObject m_EndTexture;
	public float m_EndTextureTimmerSet = 0.2f;
	private float m_EndTextureTimmer = 0.0f;
	public GameObject[] m_EndTextrues;
	public GameObject m_LiangTitle;
	public float m_LiangTimmerSet = 0.5f;
	private float m_LiangTimmer = 0.0f;
	public Animator m_Wuya;
}