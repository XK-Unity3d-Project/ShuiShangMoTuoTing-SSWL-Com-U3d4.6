using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
	public static float m_pTopSpeed = 100.0f;
	private float throttle = 0.0f;
	public bool canDrive = true;
	public WheelCollider m_wheel;
	public Transform m_pLookTarget;
	public Transform m_massCenter;
	public Transform TestCube;
	bool IsIntoFeiBan;
	public bool m_IsFinished = false;
	public float m_timmerFinished = 0.0f;
	public ColorCorrectionCurves m_ColorEffect;
	public UIController m_UIController;
	public float m_distance = 0.0f;
	private Vector3 PosRecord;
	public static float m_LimitSpeed = 10.0f;
	public float m_StopTimmerSet = 5.0f;
	public static PlayerController Instance = null;
	public bool m_IsErrorDirection = false;
	public  bool m_IsInWarter = false;
	public  bool m_IsOnRoad = false;
	public Transform m_pChuan;

	//feiyuepubu
	public static bool m_IsPubu = false;
	private float m_pubuTimmer = 0.0f;
	public static float m_PubuPower = 2000.0f;
	public float m_HitPower = 2000.0f;
	public float m_GravitySet = -2000.0f;

	//yangjiao
	public float[] m_XangleSet;
	public float[] m_XangleSpeedSet;

	//fujiao
	public float m_XangleFuchongMax = 30.0f;
	public float m_XangleFuchongTime = 2.0f;
	private float m_ResetPlayerTimmer = 0.0f;
	public float m_ResetPlayerTimeSet = 5.0f;
	public GameObject m_OutHedao;
	public GameObject m_ErrorDirection;
	private RaycastHit hit;
	private  LayerMask mask;
	public float m_OffSet = 0.5f;

	//qingjiao
	public Transform m_ForfwardPos;
	public Transform m_BehindPos;
	private Vector3 m_ForwradHitPos;
	private Vector3 m_BehindHitPos;
	public float timmerstar = 0.0f;

	//lujingchuli
	public static Transform[] PathPoint;
	public Transform Path;
	public int PathNum = 0;

	//jingtoumohu
	public RadialBlur m_RadialBlurEffect;
	public float m_SpeedForEffectStar = 0.0f;
	public float m_ParameterForEfferct = 1.0f;

	//yangjiaokongzhi
	public float m_SpeedForXangle = 0.0f;
	public float m_ParameterForXangle = 1.0f;

	//zuoyouxuanzhuansudu
	public float m_ParameterForRotate = 50.0f;

	//zuoyouqingjiao
	public float m_SpeedForZangle = 0.0f;
	public float m_ParameterForZangle = 1.0f;

	//shuihua
	public float m_speedForshuihua = 0.0f;
	public GameObject[] m_partical;
	private bool m_IsOffShuihua = false;

	public CameraShake m_CameraShake;
	public CameraSmooth m_CameraSmooth;
//	private float m_SpeedRecord = 0.0f;

	private Vector3 m_HitDirection = Vector3.forward;

	public AudioSource m_HitStone;
	public GameObject m_HitEffectObj;
	public AudioSource m_HitWater;
	public AudioSource m_YinqingAudio;
	public AudioSource m_ShuihuaAudio;
	public AudioSource m_BeijingAudio;
	public AudioSource m_ErrorDirectionAudio;
	public AudioSource m_HuanjingSenlin;
	public AudioSource m_HuanjingShuiliu;
	public AudioSource m_FeibanAudio;
	public GameObject m_FeibanEffectObj;
	private Vector3 m_WaterDirection;
	private Vector3 m_OldWaterDirection;
	private bool m_HasChanged = false;

	//zhiwujiansu
	private bool m_IsInZhiwuCollider = false;
	public float m_ParameterForZhiwu = 1.0f;
	private Vector3 m_SpeedForZhiwu;

	private bool m_CanDrive = true;

	public GameObject m_HitWaterParticle;
	public float m_BaozhaForward = 0.0f;
	public float m_BaozhaUp = 0.0f;

	public Animator m_PlayerAnimator;
	private bool m_IsJiasu = false;
	public float m_JiasuTimeSet = 3.0f;
	private float m_JiasuTimmer = 0.0f;
	public GameObject m_JiasuPartical;
	public AudioSource m_JiasuAudio;
	public AudioSource m_EatJiasuAudio;
	public float m_JiasuTopSpeed = 0.0f;
	public GameObject m_JiashiGameObject;

	public GameObject m_JiashiPartical;
	public AudioSource m_JiashiAudio;
	public AudioSource m_EatJiashiAudio;
	public AudioSource LaBaAudio;

	void Start()
	{
		PlayerMinSpeedVal = (float)ReadGameInfo.GetInstance().ReadPlayerMinSpeedVal();
		Loading.m_HasBegin = false;
		pcvr.ShaCheBtLight = StartLightState.Liang;
		pcvr.IsSlowLoopCom = false;
		pcvr.CountFXZD = 0;
		pcvr.CountQNZD = 0;
		pcvr.OpenFangXiangPanPower();
		Screen.showCursor = false;
		Time.timeScale = 1f;

		pcvr.GetInstance();
		//m_pTopSpeed = Convert_Miles_Per_Hour_To_Meters_Per_Second(m_pTopSpeed);
		rigidbody.centerOfMass = m_massCenter.localPosition;
		PosRecord = transform.position;
		mask = 1<<( LayerMask.NameToLayer("shexianjiance"));

		PathPoint = new Transform[Path.childCount];
//		Debug.Log (Path.childCount);
		for(int i = 0;i<Path.childCount;i++)
		{
			string str = (i+1).ToString();
			PathPoint[i] = Path.FindChild(str);
		}
		transform.position = PathPoint[0].position;
		transform.eulerAngles = new Vector3(PathPoint[0].eulerAngles.x,PathPoint[0].eulerAngles.y,PathPoint[0].eulerAngles.z);
		m_WaterDirection = m_OldWaterDirection = PathPoint[1].position - PathPoint[0].position;
//		m_SpeedRecord = rigidbody.velocity.magnitude;
		Instance = this;
		//InputEventCtrl.GetInstance().ClickShaCheBtEvent += ClickShaCheBtEvent;
		InputEventCtrl.GetInstance().ClickLaBaBtEvent += ClickLaBaBtEvent;
		Invoke("DelayCallClickShaCheBtEvent", 0.5f);

        StartCoroutine(GameObjectHide());   //gzknu
	}

	public static int GameGradeVal = 2;
    IEnumerator GameObjectHide()        //gzknu
    {
        yield return new WaitForSeconds(1.0f);

		GameGradeVal = PlayerPrefs.GetInt("Grade");
		switch (GameGradeVal)
        {
            case 1: //¼òµ¥
                {
                    string[] ObjNameToHide = {
                    "Obstacle/Stone_1/_newCreation_Tris537_201",
                    "Obstacle/Stone_1/_newCreation_Tris601_p",
                    "Obstacle/Stone_2/UC_SmallRock_05_p",
                    "Obstacle/Stone_2/_newCreation_Tris537_p",
                    "Obstacle/wood_3/_newCreation_Tris537_p",
                    "Obstacle/Stone_3/O2/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris601_p",

                    "Obstacle/wood_2",
                    "Obstacle/wood_3",
                    "Obstacle/Stone_4",
                    "Obstacle/Stone_5",
                    "Obstacle/Stone_6",
                    "Obstacle/Stone_7",
                    "Obstacle/Stone_8",
                    "Obstacle/Stone_9",
                    "Obstacle/Stone_10",

                                              };

                    foreach (string go in ObjNameToHide)
                    {
                        GameObject game_obj = GameObject.Find(go);
                        if (game_obj != null)
                        {
                            game_obj.SetActive(false);
                        }
                    }
                }
                break;
            case 2: //Õý³£
                {
                    string[] ObjNameToHide = {
                    "Obstacle/Stone_1/_newCreation_Tris537_201",
                    "Obstacle/Stone_1/_newCreation_Tris601_p",
                    "Obstacle/Stone_2/UC_SmallRock_05_p",
                    "Obstacle/Stone_2/_newCreation_Tris537_p",
                    "Obstacle/wood_3/_newCreation_Tris537_p",
                    "Obstacle/Stone_3/O2/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris601_p",

                                              };

                    foreach (string go in ObjNameToHide)
                    {
                        GameObject game_obj = GameObject.Find(go);
                        if (game_obj != null)
                        {
                            game_obj.SetActive(false);
                        }
                    }
                }

                break;
            case 3: //À§ÄÑ
                //Ô­°æ
                break;

            default:
                break;
        }

    }

	void DelayCallClickShaCheBtEvent()
	{
		ClickShaCheBtEvent(ButtonState.UP);
	}

	public static PlayerController GetInstance()
	{
		return Instance;
	}

	//bool IsClickShaCheBt;
	void  ClickShaCheBtEvent(ButtonState val)
	{
		if (Application.loadedLevel != 1) {
			return;
		}

//		if (val == ButtonState.DOWN) {
			//IsClickShaCheBt = true;
			//pcvr.ShaCheBtLight = StartLightState.Liang;
//		}
//		else {
//			IsClickShaCheBt = false;
//			pcvr.ShaCheBtLight = StartLightState.Shan;
//		}
	}

	void ClickLaBaBtEvent(ButtonState val)
	{
		if (val != ButtonState.DOWN) {
			LaBaAudio.Stop();
			return;
		}
		LaBaAudio.Play();
	}

	void Update () 
	{
		if(timmerstar<5.0f)
		{
			timmerstar+=Time.deltaTime;
		}
		else
		{
			if(SpeedObj > 105f && !m_IsFinished)
			{
				if (!m_IsHitshake) {
					if (pcvr.m_IsOpneLeftQinang || pcvr.m_IsOpneRightQinang) {
						pcvr.m_IsOpneForwardQinang = false;
						pcvr.m_IsOpneBehindQinang = false;
					}
					else {
						pcvr.m_IsOpneForwardQinang = true;
						pcvr.m_IsOpneBehindQinang = false;
					}
				}
			}
			else
			{
				if (!m_IsHitshake) {
					pcvr.m_IsOpneForwardQinang = false;
					pcvr.m_IsOpneBehindQinang = false;
				}
			}
			if(!m_BeijingAudio.isPlaying)
			{
				m_BeijingAudio.Play();
			}
			CalculateState();
			if(!m_IsFinished && !m_UIController.m_IsGameOver )
			{
				UpdateCameraEffect();
				UpdateShuihua();
//				m_SpeedRecord = rigidbody.velocity.magnitude;
				m_YinqingAudio.volume = rigidbody.velocity.magnitude*3.6f/120.0f;
				ResetPlayer();
				OnHitShake();
			}
			else
			{
				m_HuanjingSenlin.Stop();
				m_HuanjingShuiliu.Stop();
				m_ShuihuaAudio.Stop();
				m_YinqingAudio.Stop();
				if(m_UIController.m_IsGameOver)
				{					
					m_BeijingAudio.Stop();
				}
				m_ErrorDirectionAudio.Stop();
				m_OutHedao.SetActive(false);
				m_ErrorDirection.SetActive(false);
				m_timmerFinished+=Time.deltaTime;
				if(m_timmerFinished>1.5f)
				{
					m_ColorEffect.saturation = 0.0f;
				}
			}
			float length = Vector3.Distance(PosRecord,transform.position);
			m_distance+=length;
			PosRecord = transform.position;
			if(!m_CanDrive && canDrive)
			{
				m_IsHitshake = true;
				m_PlayerAnimator.SetTrigger("IsZhuang");
				m_CameraShake.setCameraShakeImpulseValue();
				if(m_IsInWarter && !m_IsOnRoad)
				{
					m_HitWater.Play();
					GameObject Tobject = (GameObject)Instantiate(m_HitWaterParticle,transform.position+transform.forward*m_BaozhaForward+Vector3.up*m_BaozhaUp,transform.rotation);
					Destroy(Tobject,0.5f);
				}
				else
				{
					m_HitStone.Play();
				}
			}
			m_CanDrive = canDrive;
		}
	}

	void FixedUpdate()
	{		
		if(!m_IsFinished && !m_UIController.m_IsGameOver && timmerstar >=5.0f)
		{
			//Debug.Log("rigidbody.velocity.magnitude*3.6f" + rigidbody.velocity.magnitude*3.6f);
			GetInput();
			m_pLookTarget.eulerAngles = new Vector3(0.0f,transform.eulerAngles.y,0.0f);
			CalculateEnginePower(canDrive);
		}
		if(!m_IsFinished && !m_UIController.m_IsGameOver)
		{
			if(m_IsInWarter && rigidbody.velocity.magnitude*3.6f < m_LimitSpeed && canDrive)
			{
//				Debug.Log("1111111111111111");
				m_OldWaterDirection = Vector3.Lerp(m_OldWaterDirection,m_WaterDirection,Time.deltaTime);
				rigidbody.velocity = m_OldWaterDirection.normalized*m_LimitSpeed/3.6f;
			}
		}
		if(m_IsJiasu)
		{
			m_JiasuTimmer+=Time.deltaTime;
			if(m_JiasuTimmer<m_JiasuTimeSet)
			{
				rigidbody.velocity = 1.4f*transform.forward*m_JiasuTopSpeed/3.6f;
			}
			else
			{
				m_IsJiasu =	false;
				m_JiasuTimmer = 0.0f;
			}
		}
	}

	float Convert_Miles_Per_Hour_To_Meters_Per_Second(float value)
	{
		return value * 0.44704f;
	}
	
	float mSteer;
	float mSteerTimeCur;
	float SteerOffset = 0.05f;
	void GetInput()
	{
		throttle = pcvr.mGetPower;
//		if (!IsClickShaCheBt) {
//			throttle = pcvr.mGetPower;
//		}
//		else {
//			throttle = 0f;
//			if (!m_IsJiasu && !IsIntoFeiBan) {
//				rigidbody.velocity =  Vector3.Lerp(rigidbody.velocity, Vector3.zero, Time.deltaTime * 3f);
//			}
//		}
		mSteer = pcvr.mGetSteer;

		if (mSteer < -SteerOffset)
		{
			m_PlayerAnimator.SetBool("IsTurnleft",true);
			if (SpeedObj > 15f && !m_IsHitshake) {
				pcvr.m_IsOpneRightQinang = true;
			}
		}
		else
		{
			m_PlayerAnimator.SetBool("IsTurnleft",false);
			if (!m_IsHitshake) {
				pcvr.m_IsOpneRightQinang = false;
			}
		}

		if(mSteer > SteerOffset)
		{
			m_PlayerAnimator.SetBool("IsTurnRight",true);
			if (SpeedObj > 15f && !m_IsHitshake) {
				pcvr.m_IsOpneLeftQinang = true;
			}
		}
		else
		{
			m_PlayerAnimator.SetBool("IsTurnRight",false);
			if (!m_IsHitshake) {
				pcvr.m_IsOpneLeftQinang = false;
			}
		}
		//if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		if (Mathf.Abs(mSteer) < SteerOffset)
		{
			m_PlayerAnimator.SetBool("IsRoot",true);
		}
		else
		{
			m_PlayerAnimator.SetBool("IsRoot",false);
		}
		if(canDrive && !m_IsPubu)
		{
			if(Physics.Raycast(m_ForfwardPos.position,-Vector3.up,out hit,100.0f,mask.value))
			{
				//Debug.DrawLine(m_ForfwardPos.position,hit.point,Color.red);
				m_ForwradHitPos = hit.point;
			}
			if(Physics.Raycast(m_BehindPos.position,-Vector3.up,out hit,100.0f,mask.value))
			{
				//Debug.DrawLine(m_BehindPos.position,hit.point,Color.red);
				m_BehindHitPos = hit.point;
			}
			float ytemp = Mathf.Abs( m_BehindHitPos.y - m_ForwradHitPos.y);
			if(ytemp<=5.0f)
			{
				Vector3 CurrentDirection = Vector3.Normalize(m_ForwradHitPos - m_BehindHitPos);
				transform.forward = Vector3.Lerp(transform.forward, CurrentDirection,10.0f*Time.deltaTime);
			}
		}

		//chuantouyangjiao
		if(rigidbody.velocity.magnitude*3.6f > m_SpeedForXangle && rigidbody.velocity.magnitude*3.6f <90.0f)
		{
			m_pChuan.localEulerAngles = new Vector3(-m_ParameterForXangle * rigidbody.velocity.magnitude*3.6f * rigidbody.velocity.magnitude*3.6f 
				,m_pChuan.localEulerAngles.y,m_pChuan.localEulerAngles.z);
		}
		else if(rigidbody.velocity.magnitude*3.6f >=90.0f)
		{
			m_pChuan.localEulerAngles = new Vector3(-m_ParameterForXangle*90.0f*90.0f
			                                        ,m_pChuan.localEulerAngles.y,m_pChuan.localEulerAngles.z);
		}
		if(!canDrive)
		{
			//Debug.Log("transform.localEulerAngles.x" + transform.localEulerAngles.x);
			if(transform.localEulerAngles.x < m_XangleFuchongMax || (transform.localEulerAngles.x >= 270.0f && transform.localEulerAngles.x <= 360.0f))
			{
				transform.Rotate(new Vector3(Time.deltaTime*m_XangleFuchongMax/m_XangleFuchongTime,0.0f,0.0f));
			}
			else
			{
				transform.localEulerAngles = new Vector3(42.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
			}
			//Debug.Log("transform.localEulerAngles.x" + transform.localEulerAngles.x);
			//transform.localEulerAngles = new Vector3(30.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
		}
		if(Mathf.Abs(mSteer) < 0.05f)
		{
			mSteer = 0f;
		}

		float rotSpeed = m_ParameterForRotate * mSteer * Time.smoothDeltaTime;
		transform.Rotate(0, rotSpeed, 0);
		float angleZ = 0.0f;
		if(rigidbody.velocity.magnitude*3.6f >= m_SpeedForZangle)
		{
			angleZ = -mSteer * m_ParameterForZangle* rigidbody.velocity.magnitude*3.6f*rigidbody.velocity.magnitude*3.6f;
			if(angleZ < -42f)
			{
				angleZ = -42f;
			}
			else if(angleZ > 42f)
			{
				angleZ = 42f;
			}
		}
		m_pChuan.localEulerAngles = new Vector3(m_pChuan.localEulerAngles.x,m_pChuan.localEulerAngles.y,angleZ);
	}

	private bool m_hasplay = false;
	void CalculateState()
	{
		if(Physics.Raycast(m_massCenter.position+Vector3.up*10.0f,-Vector3.up,out hit,100.0f,mask.value))
		{
			if(Vector3.Distance(m_massCenter.position,hit.point) >m_OffSet)
			{
				//Debug.Log("Vector3.Distance(m_massCenter.position,hit.point)" + Vector3.Distance(m_massCenter.position,hit.point));
				if(!m_FeibanAudio.isPlaying && !m_hasplay)
				{
					m_FeibanAudio.Play();
					Instantiate(m_FeibanEffectObj,transform.localPosition,transform.rotation);
					m_hasplay = true;
				}
				canDrive = false;
				m_IsOffShuihua = true;
				if(Vector3.Angle(m_HitDirection,Vector3.forward) >= -0.01f && Vector3.Angle(m_HitDirection,Vector3.forward)<=0.01f)
				{
					m_HitDirection = transform.forward;
				}
			}
			else
			{
				m_hasplay = false;
				m_HitDirection = Vector3.forward;
				m_IsOffShuihua = false;
				canDrive = true;
				if(m_pubuTimmer > 1.0f)
				{
					m_IsPubu = false;
					m_pubuTimmer = 0.0f;
				}
			}
		}
		if(m_IsOnRoad && !m_IsInWarter)
		{
			m_IsOffShuihua = true;
		}
	}

	float SpeedObj;
	void OnGUI()
	{
		float wVal = Screen.width;
		float hVal = 20f;
		/*string strC = "m_IsOpneForwardQinang "+pcvr.m_IsOpneForwardQinang
			+", m_IsOpneBehindQinang "+pcvr.m_IsOpneBehindQinang
				+", m_IsOpneLeftQinang "+pcvr.m_IsOpneLeftQinang
				+", m_IsOpneRightQinang "+pcvr.m_IsOpneRightQinang;
		GUI.Box(new Rect(0f, hVal * 2f, wVal, hVal), strC);*/

		if (pcvr.IsJiOuJiaoYanFailed) {
			//JiOuJiaoYanFailed
			string jiOuJiaoYanStr = "*********************************************************\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
				+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68dkfgt4saf4JOJYStr45dfssd\n"
				+ "*********************************************************";
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), jiOuJiaoYanStr);
		}
		else if (pcvr.IsJiaMiJiaoYanFailed) {
			
			string JMJYStr = "*********************************************************\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
				+ "gh4j1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4saf4JMJYStr45dfssd\n"
				+ "*********************************************************";
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), JMJYStr);
		}

		float sp = rigidbody.velocity.magnitude * 3.6f;
		sp = Mathf.Floor( sp );
		float dSpeed = SpeedObj - sp;
		if (dSpeed > 30f) {
			m_IsHitshake = true;
			//pcvr.GetInstance().OpenFangXiangPanZhenDong();
		}
		SpeedObj = sp;

		if (!pcvr.bIsHardWare || pcvr.IsTestGame) {
			string strA = sp.ToString() + "km/h";
			GUI.Box(new Rect(0f, 0f, wVal, hVal), strA);
			
			string strB = "throttle "+throttle.ToString("F3");
			GUI.Box(new Rect(0f, hVal, wVal, hVal), strB);
		}
	}

	public static float PlayerMinSpeedVal = 80f;
	void CalculateEnginePower(bool canDrive)
	{
		if(throttle > 0f && SpeedObj <= m_pTopSpeed && !m_IsPubu) {
			float speedVal = (m_pTopSpeed * throttle);
			float tmp = (m_pTopSpeed - PlayerMinSpeedVal) / (1f - pcvr.YouMemnMinVal);
			speedVal = m_pTopSpeed - (1f - throttle) * tmp;
			speedVal = speedVal < PlayerMinSpeedVal ? PlayerMinSpeedVal : speedVal;
            //			if (!pcvr.bIsHardWare) {
            //				speedVal = 80f; //test
            //			}
            speedVal /= 3.2f;   //gzkun//3.6f;
			rigidbody.velocity = speedVal * transform.forward;
		}

		if(m_IsPubu) {
			m_pubuTimmer+=Time.deltaTime;
			float throttleForce = rigidbody.mass * m_PubuPower;
			rigidbody.AddForce(transform.forward * Time.deltaTime * (throttleForce));
		}
		if(!canDrive && !m_IsPubu)
		{
//			float throttleForce = rigidbody.mass * m_HitPower;
			rigidbody.AddForce(Vector3.up * m_GravitySet*rigidbody.mass*Time.deltaTime);
		}
		if(m_IsInZhiwuCollider)
		{
			if(rigidbody.velocity.magnitude>15.0f)
			{
				rigidbody.velocity =  transform.forward* m_SpeedForZhiwu.magnitude * m_ParameterForZhiwu;
//				Debug.Log("rigidbody.velocity" + rigidbody.velocity.magnitude);
			}
			else
			{
				rigidbody.velocity = transform.forward*15.0f/3.6f;
			}
		}

		if (IsIntoFeiBan) {
			if (Time.realtimeSinceStartup - TimeFeiBan > 0.8f) {
				ResetIsIntoFeiBan();
			}
			rigidbody.velocity = (transform.forward*200f)/3.6f;

			//gzknu
			if (!m_IsHitshake) {
				pcvr.m_IsOpneForwardQinang = true;
				pcvr.m_IsOpneBehindQinang = false;
			}
		}
		OnNpcHitPlayer ();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "bianxian")
		{
			m_IsJiasu = false;
		}
		if(other.tag == "finish")
		{
			TouBiInfoCtrl.IsCloseQiNang = true;
			m_IsFinished = true;
			m_PlayerAnimator.SetBool("IsFinish",true);
		}
		if(other.tag == "water")
		{
			m_IsInWarter = true;
		}
		if(other.tag == "road")
		{
			if (!m_IsOnRoad) {
				pcvr.GetInstance().OpenFangXiangPanZhenDong();
			}
			m_IsOnRoad = true;
		}
		if(other.tag == "pathpoint")
		{
			PathNum = Convert.ToInt32(other.name)-1;
			//Debug.Log("PathNum PathNum PathNum" + PathNum);
		}
		if(other.tag == "zhangai")
		{
			if(/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude*3.6f >30.0f)
			{
				IsHitRock = true;
				m_IsHitshake = true;
				m_PlayerAnimator.SetTrigger("IsZhuang");
				m_CameraShake.setCameraShakeImpulseValue();
				m_HitStone.Play();
				Instantiate(m_HitEffectObj,transform.position,transform.rotation);
			}
		}
		if(other.tag == "zhaoshi")
		{
			if(/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude*3.6f >30.0f)
			{
				m_IsHitshake = true;
				m_PlayerAnimator.SetTrigger("IsZhuang");
				m_CameraShake.setCameraShakeImpulseValue();
				//m_HitStone.Play();
				//GameObject temp = (GameObject)Instantiate(m_HitEffectObj,transform.position,transform.rotation);
			}
		}
		if(other.tag == "qiao")
		{
			if(m_IsInWarter)
			{
				IsHitRock = true;
				m_IsHitshake = true;
				m_CameraShake.setCameraShakeImpulseValue();
				m_HitStone.Play();
				Instantiate(m_HitEffectObj,other.transform.position,other.transform.rotation);
			}
		}
        //gzknu
		//if(other.tag == "zhiwu")
		//{
		//	m_IsHitshake = true;
		//	m_IsInZhiwuCollider = true;
		//	m_SpeedForZhiwu = rigidbody.velocity;
		//	//pcvr.GetInstance().OpenFangXiangPanZhenDong();
		//}
		if(other.tag == "feibananimtor")
		{
			m_PlayerAnimator.SetTrigger("IsQifei");
		}
		if(other.tag == "pubuanimtor")
		{
			m_PlayerAnimator.SetTrigger("IsDiaoluo");
		}
		if(other.tag == "npc1p")
		{
			if(/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude*3.6f >30.0f)
			{
				m_IsHitshake = true;
				m_PlayerAnimator.SetTrigger("IsZhuang");
				m_CameraShake.setCameraShakeImpulseValue();
				m_HitStone.Play();
				Instantiate(m_HitEffectObj,transform.position,transform.rotation);
			}
			npc1.m_IsHit = true;
			npc1.m_PlayerHit = transform.position;
			npc1.m_NpcPos = npc1Pos.position;
		}
		if(other.tag == "npc2p")
		{
			if(/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude*3.6f >30.0f)
			{
				m_IsHitshake = true;
				m_PlayerAnimator.SetTrigger("IsZhuang");
				m_CameraShake.setCameraShakeImpulseValue();
				m_HitStone.Play();
				Instantiate(m_HitEffectObj,transform.position,transform.rotation);
			}
			npc2.m_IsHit = true;
			npc2.m_PlayerHit = transform.position;
			npc2.m_NpcPos = npc2Pos.position;
		}
	}
	public NpcController npc1;
	public NpcController npc2;
	public Transform npc1Pos;
	public Transform npc2Pos;
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "water")
		{
			m_IsInWarter = false;
		}
		if(other.tag == "road")
		{
			m_IsOnRoad = false;
		}
		if(other.tag=="pubu")
		{
			m_HasChanged = true;
			m_CameraSmooth.PositionForward = 6.0f;
			m_CameraSmooth.PositionUp = 5.0f;
			m_CameraSmooth.speed = 5.0f;
		}
		if(other.tag == "zhiwu")
		{
			m_IsInZhiwuCollider = false;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if(other.tag == "water")
		{
			m_IsInWarter = true;
		}
		if(other.tag == "road")
		{
			m_IsOnRoad = true;
		}
		if(other.tag == "feiban")
		{
			if (!IsIntoFeiBan) {
				TimeFeiBan = Time.realtimeSinceStartup;
				IsIntoFeiBan = true;
				m_IsHitshake = true;
				//pcvr.GetInstance().OpenFangXiangPanZhenDong();
			}
		}
		if(other.tag == "dan")
		{
			m_IsHitshake = true;
			//pcvr.GetInstance().OpenFangXiangPanZhenDong();
			m_IsJiasu = true;
			m_EatJiasuAudio.Play();
			m_JiasuAudio.Play();
			GameObject temp = (GameObject)Instantiate(m_JiasuPartical,other.transform.position,other.transform.rotation);
			Destroy(temp,0.5f);
			Destroy(other.gameObject);
		}
		if(other.tag == "zhong" && !m_UIController.m_IsGameOver)
		{
			m_IsHitshake = true;
			//pcvr.GetInstance().OpenFangXiangPanZhenDong();
			GameObject temp = (GameObject)Instantiate(m_JiashiPartical,other.transform.position,other.transform.rotation);
			Destroy(other.gameObject);
			Destroy(temp,0.5f);
			m_EatJiashiAudio.Play();
			m_JiashiAudio.Play();
			m_JiashiGameObject.SetActive(true);
		}
		if(other.tag == "pubu" && !m_UIController.m_IsGameOver)
		{
			if(!m_HasChanged)
			{
				m_CameraSmooth.PositionForward = -1.0f;
				m_CameraSmooth.PositionUp = test;
				m_CameraSmooth.speed = 300.0f;
			}
		}
//		if(other.tag == "zhiwu")
//		{
//			m_IsInZhiwuCollider = true;
//		}
	}
	public float test = 15.0f;
	void ResetPlayer()
	{
		if(PathNum == PathPoint.Length - 1)
		{
			m_WaterDirection = PathPoint[0].position - PathPoint[PathNum].position;
		}
		else
		{
			m_WaterDirection = PathPoint[PathNum+1].position - PathPoint[PathNum].position;
		}
		float angle = Vector3.Angle(m_WaterDirection,transform.forward);
		if(Mathf.Abs(angle)>=90.0f)
		{
			m_IsErrorDirection = true;
		}
		if(Mathf.Abs(angle)<90.0f)
		{
			m_IsErrorDirection = false;
		}

		if(m_IsOnRoad && !m_IsInWarter || m_IsErrorDirection)
		{
			if(!m_ErrorDirectionAudio.isPlaying)
			{
				m_ErrorDirectionAudio.Play();
			}
			if(m_IsOnRoad && !m_IsInWarter)
			{
				if(!m_OutHedao.activeSelf)
				{
					m_OutHedao.SetActive(true);
				}
			}
			else
			{
				m_OutHedao.SetActive(false);
			}
			if(m_IsErrorDirection)
			{
				if(!m_OutHedao.activeSelf && !m_ErrorDirection.activeSelf)
				{
					m_ErrorDirection.SetActive(true);
				}
				else if(m_OutHedao.activeSelf)
				{
					m_ErrorDirection.SetActive(false);
				}
			}
			m_ResetPlayerTimmer+=Time.deltaTime;
		}
		else/* if(m_IsInWarter)*/
		{
			m_ErrorDirectionAudio.Stop();
			m_OutHedao.SetActive(false);
			m_ErrorDirection.SetActive(false);
			m_ResetPlayerTimmer = 0.0f;
		}
		if(m_ResetPlayerTimmer>= m_ResetPlayerTimeSet)
		{
			//chonghzi
			m_OutHedao.SetActive(false);
			m_ErrorDirection.SetActive(false);
			m_ResetPlayerTimmer = 0.0f;
			m_IsOnRoad = false;
			m_IsInWarter = false;
			transform.position = PathPoint[PathNum].position;
			transform.localEulerAngles = PathPoint[PathNum].localEulerAngles;
		}
	}
	void UpdateCameraEffect()
	{
		if(rigidbody.velocity.magnitude*3.6f >= m_SpeedForEffectStar)
		{
			m_RadialBlurEffect.SampleStrength = m_ParameterForEfferct*rigidbody.velocity.magnitude*3.6f*3.6f*rigidbody.velocity.magnitude;
		}
		else
		{
			m_RadialBlurEffect.SampleStrength = 0.0f;
		}
	}
	void UpdateShuihua()
	{

		if(rigidbody.velocity.magnitude*3.6f >= m_speedForshuihua && !m_IsOffShuihua)
		{
			m_partical[0].SetActive(true);
			m_partical[1].SetActive(true);
			if(!m_ShuihuaAudio.isPlaying)
			{
				m_ShuihuaAudio.Play();
			}
			m_ShuihuaAudio.volume =rigidbody.velocity.magnitude*3.6f/120.0f;
		}
		else
		{
			m_partical[0].SetActive(false);
			m_partical[1].SetActive(false);
			m_ShuihuaAudio.Stop();
		}
		if(m_IsOffShuihua)
		{
			m_partical[2].SetActive(false);
		}
		else
		{
			m_partical[2].SetActive(true);
		}
	}
	float m_HitshakeTimmerSet = 0.2f;
	private float m_HitshakeTimmer = 0.0f;
	private bool m_IsHitshake = false;
	static bool IsHitRock = false;
	void OnHitShake()
	{
		if(m_IsHitshake)
		{
			if(m_HitshakeTimmer<m_HitshakeTimmerSet)
			{
				TimeHitRock = 0f;
				m_HitshakeTimmer+=Time.deltaTime;
				if(m_HitshakeTimmer<m_HitshakeTimmerSet*0.25f || (m_HitshakeTimmer>=m_HitshakeTimmerSet*0.5f && m_HitshakeTimmer<m_HitshakeTimmerSet*0.75f))
				{
					pcvr.m_IsOpneForwardQinang = IsHitRock;
					pcvr.m_IsOpneBehindQinang = IsHitRock;
					pcvr.m_IsOpneLeftQinang = false;
					pcvr.m_IsOpneRightQinang = true;
				}
				else if((m_HitshakeTimmer>=m_HitshakeTimmerSet*0.25f &&m_HitshakeTimmer<m_HitshakeTimmerSet*0.5f) || m_HitshakeTimmer>=m_HitshakeTimmerSet*0.75f)
				{
					pcvr.m_IsOpneForwardQinang = IsHitRock;
					pcvr.m_IsOpneBehindQinang = IsHitRock;
					pcvr.m_IsOpneLeftQinang = true;
					pcvr.m_IsOpneRightQinang = false;
				}
			}
			else
			{
				TimeHitRock+=Time.deltaTime;
				pcvr.m_IsOpneLeftQinang = false;
				pcvr.m_IsOpneRightQinang = false;
				if (TimeHitRock >= 2f) {
					pcvr.CountQNZD++;
					TimeHitRock = 0f;
					m_HitshakeTimmer = 0.0f;
					m_IsHitshake = false;
					IsHitRock = false;
					pcvr.m_IsOpneForwardQinang = false;
					pcvr.m_IsOpneBehindQinang = false;
//					pcvr.m_IsOpneLeftQinang = false;
//					pcvr.m_IsOpneRightQinang = false;
				}
			}
			pcvr.GetInstance().OpenFangXiangPanZhenDong();
		}
	}
	static float TimeHitRock;

	void OnNpcHitPlayer()
	{
		if(npc1.m_IsHit)
		{
			if(npc1.m_HitTimmer<0.4f)
			{
				//Debug.Log("1111111111111111111111");
				rigidbody.AddForce(Vector3.Normalize(npc1.m_PlayerHit - npc1.m_NpcPos )*80000.0f,ForceMode.Force);
			}
		}
		if(npc2.m_IsHit)
		{
			if(npc2.m_HitTimmer<0.4f)
			{
				//Debug.Log("22222222222222222222222222");
				rigidbody.AddForce(Vector3.Normalize(npc2.m_PlayerHit - npc2.m_NpcPos )*80000.0f,ForceMode.Force);
			}
		}
	}
	float TimeFeiBan;
	void ResetIsIntoFeiBan()
	{
		IsIntoFeiBan = false;
	}
}