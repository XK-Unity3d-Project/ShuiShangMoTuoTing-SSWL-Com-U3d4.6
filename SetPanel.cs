#define GAME_GRADE
//gzknu
using UnityEngine;
using System.Collections;
using System;

public class SetPanel : MonoBehaviour
{
	//zhujiemian
//	private bool m_IsZhujiemian = true;
	public GameObject m_ZhujiemianObject;
	public Transform m_ZhujiemianXingXing;
	private int m_IndexZhujiemian = 0;
	public UILabel m_CoinForStar;
	public UILabel PlayerMinSpeed;
	public UITexture m_GameModeDuigou1;
	public UITexture m_GameModeDuigou2;
	public UITexture[] GameGradeDuiGou;
	public UILabel InsertCoinNumLabel;
	public UILabel BtInfoLabel;
	public UILabel YouMenInfoLabel;
	public UILabel FangXiangInfoLabel;
	private int m_InserNum = 0;
	public UITexture JiaoZhunTexture;
	public Texture[] JiaoZhunTextureArray;
	bool IsInitJiaoZhunPcvr;
	int JiaoZhunCount;
	GameObject JiaoZhunObj;
	public static bool IsOpenSetPanel = false;
	int GameAudioVolume;
	void Start () 
	{
        ReadGameInfo conf = ReadGameInfo.GetInstance();

        XkGameCtrl.IsLoadingLevel = false;
		GameAudioVolume = conf.ReadGameAudioVolume();
		GameAudioVolumeLB.text = GameAudioVolume.ToString();

		IsOpenSetPanel = true;
		CloseAllQiNang();
		pcvr.ShaCheBtLight = StartLightState.Mie;
		pcvr.CloseFangXiangPanPower();
		pcvr.IsSlowLoopCom = false;
		m_InserNum = Convert.ToInt32(conf.ReadInsertCoinNum());
		UpdateInsertCoin();

		BtInfoLabel.text = "";
		m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f,212.0f,0.0f);
		string GameMode = conf.ReadGameStarMode();
		if (GameMode == "" || GameMode == null) {
			GameMode = "oper";
		}

		m_CoinForStar.text = conf.ReadStarCoinNumSet();
		int minSpeedPlayer = (int)conf.ReadPlayerMinSpeedVal();
		PlayerMinSpeed.text = minSpeedPlayer.ToString();
		if(GameMode == "oper")
		{
			m_GameModeDuigou1.enabled = true;
			m_GameModeDuigou2.enabled = false;
		}
		else
		{
			m_GameModeDuigou1.enabled = false;
			m_GameModeDuigou2.enabled = true;
		}

		if (JiaoZhunTexture != null) {
			JiaoZhunObj = JiaoZhunTexture.gameObject;
			JiaoZhunObj.SetActive(false);
		}
		InitSteerForceInfo();

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
		//InputEventCtrl.GetInstance().ClickShaCheBtEvent += ClickShaCheBtEvent;
		InputEventCtrl.GetInstance().ClickLaBaBtEvent += ClickLaBaBtEvent;
		InputEventCtrl.GetInstance().ClickCloseDongGanBtEvent += ClickCloseDongGanBtEvent;

        if (conf.Grade == 0)
        {
            conf.Grade = 2;
        }

        switch (conf.Grade) {
            case 1:
                GameGradeDuiGou[0].enabled = true;
                GameGradeDuiGou[1].enabled = false;
                GameGradeDuiGou[2].enabled = false;
                break;
            case 3:
                GameGradeDuiGou[0].enabled = false;
                GameGradeDuiGou[1].enabled = false;
                GameGradeDuiGou[2].enabled = true;
                break;
            case 2:
            default: // 默认设置为普通难度
                GameGradeDuiGou[0].enabled = false;
                GameGradeDuiGou[1].enabled = true;
                GameGradeDuiGou[2].enabled = false;
                break;
        }
    }

	void Update () 
	{
		if (pcvr.bIsHardWare) {
			if (GlobalData.CoinCur > m_InserNum) {
				m_InserNum = GlobalData.CoinCur - 1;
				OnClickInsertBt();
			}

			YouMenInfoLabel.text = pcvr.BikePowerCur.ToString();
			FangXiangInfoLabel.text = pcvr.SteerValCur.ToString();

			if (!IsInitJiaoZhunPcvr) {
				if (pcvr.mGetPower > pcvr.YouMemnMinVal) {				
					YouMenInfoLabel.text += ", Throttle Response";
				}

				float offsetSteer = 0.05f;
				if (pcvr.mGetSteer < -offsetSteer) {
					FangXiangInfoLabel.text += ", Turn Left";
				}
				else if (pcvr.mGetSteer > offsetSteer) {
					FangXiangInfoLabel.text += ", Turn Right";
				}
				else {
					FangXiangInfoLabel.text += ", Turn Middle";
				}
			}
		}
		else {
			if (Input.GetKeyDown(KeyCode.T)) {
				OnClickInsertBt();
			}

			int val = (int)(pcvr.mGetSteer * 100);
			FangXiangInfoLabel.text = val.ToString();
			if (!IsInitJiaoZhunPcvr) {
				if (val < 0) {
					FangXiangInfoLabel.text += ", Turn Left";
				}
				else if (val > 0) {
					FangXiangInfoLabel.text += ", Turn Right";
				}
				else {
					FangXiangInfoLabel.text += ", Turn Middle";
				}
			}

			val = (int)(pcvr.mGetPower * 100);
			YouMenInfoLabel.text = val.ToString();
			if (!IsInitJiaoZhunPcvr) {
				if (val > 0) {				
					YouMenInfoLabel.text += ", Throttle Response";
				}
			}
		}
	}

	void OnClickInsertBt()
	{
		m_InserNum++;
		ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
		UpdateInsertCoin();
	}
	
	void UpdateInsertCoin()
	{
		//Debug.Log("m_InserNum "+m_InserNum);
		InsertCoinNumLabel.text = m_InserNum.ToString();
	}

	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		OnClickMoveBtInZhujiemian();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		OnClickSelectBtInZhujiemian();
	}

	void ClickStartBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			BtInfoLabel.text = "StartBtDown";
		}
		else {
			BtInfoLabel.text = "StartBtUp";
			if (IsInitJiaoZhunPcvr) {
				if (JiaoZhunCount > 2) {
					ResetJiaoZhunPcvr();
				}
				else {
					UpdataJiaoZhunTexture();
				}
			}
		}
	}

	void ClickCloseDongGanBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			BtInfoLabel.text = "DongGanBtDown";
		}
		else {
			BtInfoLabel.text = "DongGanBtUp";
		}
	}

	void ResetJiaoZhunPcvr()
	{
		if (!IsInitJiaoZhunPcvr) {
			return;
		}
		m_ZhujiemianXingXing.gameObject.SetActive(true);
		IsInitJiaoZhunPcvr = false;
		JiaoZhunObj.SetActive(false);
	}

	void InitJiaoZhunPcvr()
	{
		if (IsInitJiaoZhunPcvr) {
			return;
		}
		pcvr.GetInstance().InitFangXiangJiaoZhun();
		m_ZhujiemianXingXing.gameObject.SetActive(false);
		IsInitJiaoZhunPcvr = true;

		JiaoZhunCount = 0;
		JiaoZhunTexture.mainTexture = JiaoZhunTextureArray[0];
		JiaoZhunObj.SetActive(true);
	}

	void UpdataJiaoZhunTexture()
	{
		JiaoZhunCount++;
		JiaoZhunTexture.mainTexture = JiaoZhunTextureArray[JiaoZhunCount];
	}

#if GAME_GRADE
    void OnClickMoveBtInZhujiemian()
	{
        if (IsInitJiaoZhunPcvr)
        {
            return;
        }

        m_IndexZhujiemian++;
		if (m_IndexZhujiemian > (int)GameSet.Exit) {
            m_IndexZhujiemian = 0;
        }
		GameSetSt = (GameSet)m_IndexZhujiemian;

		switch (GameSetSt)
        {
			case GameSet.CoinStart:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, 212.0f, 0.0f);
                    break;
                }
			case GameSet.OperMode:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-640.0f, 139.0f, 0.0f);
                    break;
                }
			case GameSet.FreeMode:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-278.0f, 139.0f, 0.0f);
                    break;
                }
			case GameSet.ResetFactory:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, 66.0f, 0.0f);
                    break;
                }
            case GameSet.StartLEDLiang:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, -4.5f, 0.0f);
                    break;
                }
            case GameSet.StartLEDShan:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-385.0f, -4.5f, 0.0f);
                    break;
                }
            case GameSet.StartLEDMie:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-253.0f, -4.5f, 0.0f);
                    break;
                }
            case GameSet.CheckQiNang1:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -93.0f, 0.0f);
                    break;
                }
            case GameSet.CheckQiNang2:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -125.0f, 0.0f);
                    break;
                }
            case GameSet.CheckQiNang3:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -165.0f, 0.0f);
                    break;
                }
            case GameSet.CheckQiNang4:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -200.0f, 0.0f);
                    break;
                }
            case GameSet.GameAudioSet:
				{
                    CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-575.0f, -285.0f, 0.0f);
                    break;
                }
            case GameSet.GameAudioReset:
                {
					m_ZhujiemianXingXing.localPosition = new Vector3(-270.0f, -285.0f, 0.0f);
                    break;
                }
            case GameSet.OriginalSpeed:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, -358.0f, 0.0f);
                    break;
                }
            case GameSet.AdjustGame:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(56.0f, -81.0f, 0.0f);
                    break;
				}
			case GameSet.SteerForceShiWei:
				{
					m_ZhujiemianXingXing.localPosition = new Vector3(157.0f, -197.0f, 0.0f);
					break;
				}
			case GameSet.SteerForceGeWei:
				{
					m_ZhujiemianXingXing.localPosition = new Vector3(183.0f, -197.0f, 0.0f);
					break;
				}
            case GameSet.GradeEasy:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(35.0f, -285.0f, 0.0f);
                    break;
                }
            case GameSet.GradeNormal:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(235.0f, -285.0f, 0.0f);
                    break;
                }
            case GameSet.GradeHard:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(450.0f, -285.0f, 0.0f);
                    break;
                }
            case GameSet.Exit:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(56.0f, -358.0f, 0.0f);
                    break;
                }
        }
    }

	enum GameSet
	{
		CoinStart,
		OperMode,
		FreeMode,
		ResetFactory,
		StartLEDLiang,
		StartLEDShan,
		StartLEDMie,
		CheckQiNang1,
		CheckQiNang2,
		CheckQiNang3,
		CheckQiNang4,
		GameAudioSet,
		GameAudioReset,
		OriginalSpeed,
		AdjustGame,
		SteerForceShiWei,
		SteerForceGeWei,
		GradeEasy,
		GradeNormal,
		GradeHard,
		Exit,
	}
	GameSet GameSetSt = GameSet.CoinStart;

	int SteerForceShiWei = 2;
	int SteerForceGeWei = 5;
	public UILabel SteerForceLB;
	void InitSteerForceInfo()
	{
		ReadGameInfo conf = ReadGameInfo.GetInstance();
		SteerForceShiWei = conf.SteerForce / 10;
		SteerForceGeWei = conf.SteerForce % 10;
		SetSteerForceLBInfo(1);
	}

	void SetSteerForceShiWei()
	{
		SteerForceShiWei++;
		if (SteerForceShiWei > 8) {
			SteerForceShiWei = 0;
		}

		if (SteerForceShiWei == 8) {
			SteerForceGeWei = 0;
		}
		SetSteerForceLBInfo();
	}

	void SetSteerForceGeWei()
	{
		if (SteerForceShiWei >= 8) {
			return;
		}

		SteerForceGeWei++;
		if (SteerForceGeWei > 9) {
			SteerForceGeWei = 0;
		}
		SetSteerForceLBInfo();
	}

	void ResetSteerForce()
	{
		SteerForceShiWei = 2;
		SteerForceGeWei = 5;
		SetSteerForceLBInfo();
	}

	void SetSteerForceLBInfo(int key = 0)
	{
		if (key == 0) {
			ReadGameInfo conf = ReadGameInfo.GetInstance();
			conf.SteerForce = (SteerForceShiWei * 10) + SteerForceGeWei;
		}
		SteerForceLB.text = SteerForceShiWei.ToString() + SteerForceGeWei.ToString() + " (0-80)";
	}

    void OnClickSelectBtInZhujiemian()
    {
        ReadGameInfo conf = ReadGameInfo.GetInstance();

        switch (GameSetSt)
        {
            case GameSet.CoinStart:  // 启动币数
                {
                    int CoinNum = Convert.ToInt32(m_CoinForStar.text);
                    CoinNum++;
                    if (CoinNum > 9)
                    {
                        CoinNum = 1;
                    }
                    m_CoinForStar.text = CoinNum.ToString();
                    conf.WriteStarCoinNumSet(CoinNum.ToString());
                    break;
                }
            case GameSet.OperMode:  // 运营模式
                {
                    m_GameModeDuigou1.enabled = true;
                    m_GameModeDuigou2.enabled = false;
                    conf.WriteGameStarMode("oper");
                    break;
                }
            case GameSet.FreeMode:  // 免费模式
                {
                    m_GameModeDuigou1.enabled = false;
                    m_GameModeDuigou2.enabled = true;
                    conf.WriteGameStarMode("FREE");
                    break;
                }
            case GameSet.ResetFactory:  // 恢复出厂设置
                {
                    ResetFactory();
                    break;
                }
            case GameSet.StartLEDLiang:  // 开始按键灯 - 亮
                {
                    pcvr.StartBtLight = StartLightState.Liang;
                    break;
                }
            case GameSet.StartLEDShan:  // 开始按键灯 - 闪
                {
                    pcvr.StartBtLight = StartLightState.Shan;
                    break;
                }
            case GameSet.StartLEDMie:  // 开始按键灯 - 灭
                {
                    pcvr.StartBtLight = StartLightState.Mie;
                    break;
                }
            case GameSet.CheckQiNang1:  // 气囊信息 - 前气囊
                {
                    pcvr.m_IsOpneForwardQinang = true;
                    pcvr.m_IsOpneBehindQinang = false;
                    pcvr.m_IsOpneLeftQinang = false;
                    pcvr.m_IsOpneRightQinang = false;
                    break;
                }
			case GameSet.CheckQiNang2:  // 气囊信息 - 后气囊
                {
                    pcvr.m_IsOpneForwardQinang = false;
                    pcvr.m_IsOpneBehindQinang = true;
                    pcvr.m_IsOpneLeftQinang = false;
                    pcvr.m_IsOpneRightQinang = false;
                    break;
                }
			case GameSet.CheckQiNang3:  // 气囊信息 - 左气囊
                {
                    pcvr.m_IsOpneForwardQinang = false;
                    pcvr.m_IsOpneBehindQinang = false;
                    pcvr.m_IsOpneLeftQinang = true;
                    pcvr.m_IsOpneRightQinang = false;
                    break;
                }
			case GameSet.CheckQiNang4: // 气囊信息 - 右气囊
                {
                    pcvr.m_IsOpneForwardQinang = false;
                    pcvr.m_IsOpneBehindQinang = false;
                    pcvr.m_IsOpneLeftQinang = false;
                    pcvr.m_IsOpneRightQinang = true;
                    break;
                }
			case GameSet.GameAudioSet:  // 音量设置 - <音量值>
				{
					GameAudioVolume++;
					if (GameAudioVolume > 10) {
						GameAudioVolume = 0;
					}
					GameAudioVolumeLB.text = GameAudioVolume.ToString();
					conf.WriteGameAudioVolume(GameAudioVolume);
					break;
				}
			case GameSet.GameAudioReset:  // 音量设置 - 重置
                {
					GameAudioVolume = 7;
					GameAudioVolumeLB.text = GameAudioVolume.ToString();
					conf.WriteGameAudioVolume(GameAudioVolume);
					break;
				}
			case GameSet.OriginalSpeed:  // 初始速度
                {
                    int speedVal = Convert.ToInt32(PlayerMinSpeed.text);
                    speedVal += 10;
                    if (speedVal > 80)
                    {
                        speedVal = 0;
                    }
                    PlayerMinSpeed.text = speedVal.ToString();
                    conf.WritePlayerMinSpeedVal(speedVal);
                    break;
                }
			case GameSet.AdjustGame:  // 校准
                {
                    InitJiaoZhunPcvr();
                    break;
				}
			case GameSet.SteerForceShiWei:
				{
					SetSteerForceShiWei();
					break;
				}
			case GameSet.SteerForceGeWei:
				{
					SetSteerForceGeWei();
					break;
				}
			case GameSet.GradeEasy:  // 难度 - 简单
				{
					GameGradeDuiGou[0].enabled = true;
					GameGradeDuiGou[1].enabled = false;
					GameGradeDuiGou[2].enabled = false;
                    conf.Grade = 1;
                    break;
                }

			case GameSet.GradeNormal:  // 难度 - 正常
                {
					GameGradeDuiGou[0].enabled = false;
					GameGradeDuiGou[1].enabled = true;
					GameGradeDuiGou[2].enabled = false;
                    conf.Grade = 2;
                    break;
                }

			case GameSet.GradeHard:  // 难度 - 困难
                {
					GameGradeDuiGou[0].enabled = false;
					GameGradeDuiGou[1].enabled = false;
					GameGradeDuiGou[2].enabled = true;
                    conf.Grade = 3;
                    break;
                }

			case GameSet.Exit:  // 退出
                {
                    conf.Save();
                    CloseAllQiNang();
					pcvr.StartBtLight = StartLightState.Mie;
					XkGameCtrl.IsLoadingLevel = true;
                    IsOpenSetPanel = false;
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    Application.LoadLevel(0);
                    break;
                }
        }
    }
#endif

	public UILabel GameAudioVolumeLB;
	void CloseAllQiNang()
	{
		pcvr.m_IsOpneForwardQinang = false;
		pcvr.m_IsOpneBehindQinang = false;
		pcvr.m_IsOpneLeftQinang = false;
		pcvr.m_IsOpneRightQinang = false;
	}

	void ResetFactory()
	{
        ReadGameInfo conf = ReadGameInfo.GetInstance();
        conf.FactoryReset();

		PlayerMinSpeed.text = "0";
		m_CoinForStar.text = "1";
		m_GameModeDuigou1.enabled = true;
		m_GameModeDuigou2.enabled = false;
		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();

        if (pcvr.bIsHardWare) {
			pcvr.GetInstance().SubPlayerCoin(m_InserNum);
		}
		m_InserNum = 0;
		UpdateInsertCoin();

        switch (conf.Grade) {
            case 1:
                GameGradeDuiGou[0].enabled = true;
                GameGradeDuiGou[1].enabled = false;
                GameGradeDuiGou[2].enabled = false;
                break;
            case 3:
                GameGradeDuiGou[0].enabled = false;
                GameGradeDuiGou[1].enabled = false;
                GameGradeDuiGou[2].enabled = true;
                break;
            case 2:
            default: // 默认设置为普通难度
                GameGradeDuiGou[0].enabled = false;
                GameGradeDuiGou[1].enabled = true;
                GameGradeDuiGou[2].enabled = false;
                break;
        }
		ResetSteerForce();
    }

	void ClickShaCheBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			BtInfoLabel.text = "BrakeBtDown";
		}
		else {
			BtInfoLabel.text = "BrakeBtUp";
		}
	}

	void ClickLaBaBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			BtInfoLabel.text = "SpeakerBtDown";
		}
		else {
			BtInfoLabel.text = "SpeakerBtUp";
		}
	}

//    void OnGUI()
//    {
//        GUI.Box(new Rect(Screen.width / 1.8f, Screen.height / 1.5f, 400f, 30f), "难度(GRADE)");
//        GUI.Box(new Rect(Screen.width / 1.8f, Screen.height / 1.4f, 100f, 30f), "简单(EASY)");
//        GUI.Box(new Rect(Screen.width / 1.5f, Screen.height / 1.4f, 100f, 30f), "正常(NORMAL)");
//        GUI.Box(new Rect(Screen.width / 1.285f, Screen.height / 1.4f, 100f, 30f), "困难(HARD)");
//    }
}