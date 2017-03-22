#define SHI_ZI_QI_NANG
//#define OUT_INFO_TEST

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using SLAB_HID_DEVICE;
using System;
using System.IO;

public class pcvr : MonoBehaviour {
    static public bool bIsHardWare = true;
	static public bool IsTestGame = false;
	public static uint ShaCheCurPcvr;
	static bool IsClickLaBaBt;
	static public uint gOldCoinNum = 0;
	static private uint mOldCoinNum = 0;
	public int CoinNumCurrent = 0;
	static public bool IsCloseDongGanBtDown = false;
	static public bool bPlayerStartKeyDown = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
	public static bool IsZhenDongFangXiangPan;
	int SubCoinNum = 0;
	public static bool m_IsOpneForwardQinang = false;
	public static bool m_IsOpneBehindQinang = false;
	public static bool m_IsOpneLeftQinang = false;
	public static bool m_IsOpneRightQinang = false;
	public static bool m_IsOpneQinang1 = false;
	public static bool m_IsOpneQinang2 = false;
	public static bool m_IsOpneQinang3 = false;
	public static bool m_IsOpneQinang4 = false;
	public static uint SteerValMax = 999999;
	public static uint SteerValCen = 1765;
	public static uint SteerValMin = 0;
	public static uint SteerValCur;
	public static float mGetSteer = 0f;
	public static uint BikeShaCheCur;
	public static uint mBikePowerMin = 999999;
	public static uint mBikePowerMax = 0;
	public static float mGetPower = 0f;
	static uint BikePowerLen = 0;
	public static uint BikePowerCur;
	public static uint BikePowerOld;
	bool bIsJiaoYanBikeValue = false;
	static bool IsInitYouMenJiaoZhun = false;
	bool IsJiaoZhunFireBt;
	bool IsFanZhuangYouMen;
	static bool IsInitFangXiangJiaoZhun;
	bool IsFanZhuangFangXiang;
	int FangXiangJiaoZhunCount;
	public static uint CoinCurPcvr;
	public static uint BikePowerCurPcvr;
	public static StartLightState StartBtLight = StartLightState.Mie;
	public static StartLightState DongGanBtLight = StartLightState.Mie;
	bool IsCleanHidCoin;
	static uint BikeDirLenA;
	static uint BikeDirLenB;
	static uint BikeDirLenC;
	static uint BikeDirLenD;
	public static bool IsActiveSheCheEvent;
	static bool IsInitShaCheJiaoZhun;
	static bool IsFanZhuangShaChe;
	static uint mBikeShaCheMin = 999999;
	static uint mBikeShaCheMax = 0;
	static uint BikeShaCheLen = 1;
	bool IsPlayFangXiangPanZhenDong;
	static private pcvr Instance = null;
	static public pcvr GetInstance()
	{
		if (Instance == null) {
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<pcvr>();
			ScreenLog.init();
			if (bIsHardWare) {
				MyCOMDevice.GetInstance();
			}

			if (HardWareTest.IsTestHardWare) {
				TestComPort.GetInstance();
			}
			//ScreenLog.Log("open hid***********************");
		}
		return Instance;
	}

	static bool IsInitFangXiangPower;
	void InitFangXiangPowerOpen()
	{
		if (HardWareTest.IsTestHardWare) {
			return;
		}

		if (IsInitFangXiangPower) {
			return;
		}
		IsInitFangXiangPower = true;
		OpenFangXiangPanPower();
		//Debug.Log("*********");
		Invoke("DelayCloseFangXiangPanPower", 300f);
	}

	void DelayCloseFangXiangPanPower()
	{
		//Debug.Log("*********55555555555");
		IsInitFangXiangPower = false;
		if (Application.loadedLevel != 1) {
			CloseFangXiangPanPower();
		}
	}

	void Start()
	{
		InitJiaoYanMiMa();

        ReadGameInfo conf = ReadGameInfo.GetInstance();

        //FangXiangInfo
        SteerValMin = (uint)conf.SteerMin;
		SteerValCen = (uint)conf.SteerCenter;
        SteerValMax = (uint)conf.SteerMax;
		CheckBikeDirLen();
		
		//YouMenInfo
		mBikePowerMin = (uint)conf.ThrustMin;
        mBikePowerMax = (uint)conf.ThrustMax;
        BikePowerLen = mBikePowerMax < mBikePowerMin ? (mBikePowerMin - mBikePowerMax + 1) : (mBikePowerMax - mBikePowerMin + 1);
		BikePowerLen = Math.Max(1, BikePowerLen);

		mBikeShaCheMin = (uint)conf.BrakeMin;
		mBikeShaCheMax = (uint)conf.BrakeMax;
        BikeShaCheLen = mBikeShaCheMax < mBikeShaCheMin ? (mBikeShaCheMin - mBikeShaCheMax + 1) : (mBikeShaCheMax - mBikeShaCheMin + 1);
		BikeShaCheLen = Math.Max(1, BikeShaCheLen);
		
		InitFangXiangPowerOpen();
    }

	void Update()
	{
		if (IsTestGame  &&  Input.GetKeyUp(KeyCode.O)) {
			IsHandleDirByKey = !IsHandleDirByKey;
		}

		GetPcvrPowerVal();
		GetPcvrSteerVal();
		//GetPcvrShaCheVal();
		if (!bIsHardWare) {
			return;
		}

		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		SendMessage();
		GetMessage();
	}
	
	static byte ReadHead_1 = 0x01;
	static byte ReadHead_2 = 0x55;
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	public static bool IsOpenFangXiangPanPower = true;
	public static StartLightState ShaCheBtLight = StartLightState.Mie;
	public static void OpenFangXiangPanPower()
	{
		IsOpenFangXiangPanPower = true;
	}
	
	public static void CloseFangXiangPanPower()
	{
		if (IsInitFangXiangPower) {
			return;
		}
		IsOpenFangXiangPanPower = false;
	}

	void SendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}

		byte []buffer = new byte[MyCOMDevice.ComThreadClass.BufLenWrite];
		for (int i = 6; i < (MyCOMDevice.ComThreadClass.BufLenWrite - 2); i++) {
//			if (i >= 7 && i <= 11) {
//				continue;
//			}

//			if (i > 34 && i < 41) {
//				//方向盘信息.
//				continue;
//			}
			buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0xff);
		}
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[MyCOMDevice.ComThreadClass.BufLenWrite - 2] = WriteEnd_1;
		buffer[MyCOMDevice.ComThreadClass.BufLenWrite - 1] = WriteEnd_2;
		
		switch (StartBtLight) {
		case StartLightState.Liang:
			buffer[4] |= 0x40;
			break;
			
		case StartLightState.Shan:
			buffer[4] |= 0x40;
			break;
			
		case StartLightState.Mie:
			buffer[4] &= 0xbf;
			break;
		}
		
		switch (ShaCheBtLight) {
		case StartLightState.Liang:
			buffer[7] = 0xaa;
			break;
			
		case StartLightState.Shan:
			buffer[7] = 0x55;
			break;
			
		case StartLightState.Mie:
			buffer[7] = 0x00;
			break;
		}

		if (TouBiInfoCtrl.IsCloseDongGan || TouBiInfoCtrl.IsCloseQiNang) {
			buffer[4] <<= 4;
		}
		else {
			/*
0	气囊1：充气1、放气0	（快艇气囊1）		0x01	0xFE
1	气囊2：充气1、放气0	（快艇气囊2）		0x02	0xFD
2	气囊3：充气1、放气0	（快艇气囊3）		0x04	0xFB
3	气囊4：充气1、放气0	（快艇气囊4）		0x08	0xF7

1    2

4    3
			 */

            /*
十字型气囊            
             
        1
    4       2
		3

             */

#if SHI_ZI_QI_NANG

            if (!HardWareTest.IsTestHardWare || HardWareTest.m_IsHitshake)
            {
                if (SetPanel.IsOpenSetPanel)
                {
                    if (m_IsOpneForwardQinang)
                    {
                        buffer[4] |= 0x01;
                    }
                    else
                    {
                        buffer[4] &= 0xfe;
                    }

					if (m_IsOpneRightQinang)
					{
						buffer[4] |= 0x02;
                    }
                    else
                    {
                        buffer[4] &= 0xfd;
                    }

					if (m_IsOpneBehindQinang)
                    {
                        buffer[4] |= 0x04;
                    }
                    else
                    {
                        buffer[4] &= 0xfb;
                    }

					if (m_IsOpneLeftQinang)
                    {
                        buffer[4] |= 0x08;
                    }
                    else
                    {
                        buffer[4] &= 0xf7;
                    }

                }
                else
                {
                    if (m_IsOpneForwardQinang)
                    {
                        buffer[4] |= 0x01;
                    }
                    else
                    {
                        buffer[4] &= 0xfe;
                    }

					if (m_IsOpneRightQinang)
					{
						buffer[4] |= 0x02;
                    }
                    else
                    {
                        buffer[4] &= 0xfd;
                    }

					if (m_IsOpneBehindQinang)
                    {
                        buffer[4] |= 0x04;
                    }
                    else
                    {
                        buffer[4] &= 0xfb;
                    }

					if (m_IsOpneLeftQinang)
                    {
                        buffer[4] |= 0x08;
                    }
                    else
                    {
                        buffer[4] &= 0xf7;
                    }
                }
            }
            else
            {
                if (m_IsOpneQinang1)
                {
                    buffer[4] |= 0x01;
                }
                else
                {
                    buffer[4] &= 0xfe;
                }

                if (m_IsOpneQinang2)
                {
                    buffer[4] |= 0x02;
                }
                else
                {
                    buffer[4] &= 0xfd;
                }

                if (m_IsOpneQinang3)
                {
                    buffer[4] |= 0x04;
                }
                else
                {
                    buffer[4] &= 0xfb;
                }

                if (m_IsOpneQinang4)
                {
                    buffer[4] |= 0x08;
                }
                else
                {
                    buffer[4] &= 0xf7;
                }
            }

#else
            if (!HardWareTest.IsTestHardWare || HardWareTest.m_IsHitshake) {
				if (SetPanel.IsOpenSetPanel) {
					if (m_IsOpneForwardQinang) {
						buffer[4] |=  0x01;
					}
					else {
						buffer[4] &=  0xfe;
					}
					
					if (m_IsOpneBehindQinang) {
						buffer[4] |=  0x02;
					}
					else {
						buffer[4] &=  0xfd;
					}
					
					if (m_IsOpneLeftQinang) {
						buffer[4] |=  0x04;
					}
					else {
						buffer[4] &=  0xfb;
					}
					
					if (m_IsOpneRightQinang) {
						buffer[4] |=  0x08;
					}
					else {
						buffer[4] &=  0xf7;
					}
				}
				else {
					if (m_IsOpneForwardQinang) {
						buffer[4] |=  0x01;
						buffer[4] |=  0x02;
					}
					else {
						buffer[4] &=  0xfe;
						buffer[4] &=  0xfd;
					}
					
					if (m_IsOpneBehindQinang) {
						buffer[4] |=  0x04;
						buffer[4] |=  0x08;
					}
					else {
						buffer[4] &=  0xfb;
						buffer[4] &=  0xf7;
					}
					
					if (m_IsOpneLeftQinang) {
						buffer[4] |=  0x01;
						buffer[4] |=  0x08;
					}
					else {
						if (!m_IsOpneForwardQinang) {
							buffer[4] &=  0xfe;
							buffer[4] &=  0xf7;
						}
					}
					
					if (m_IsOpneRightQinang) {
						buffer[4] |=  0x02;
						buffer[4] |=  0x04;
					}
					else {
						if (!m_IsOpneForwardQinang) {
							buffer[4] &=  0xfd;
							buffer[4] &=  0xfb;
						}
					}
				}
			}
			else {
				
				if (m_IsOpneQinang1) {
					buffer[4] |=  0x01;
				}
				else {
					buffer[4] &=  0xfe;
				}
				
				if (m_IsOpneQinang2) {
					buffer[4] |=  0x02;
				}
				else {
					buffer[4] &=  0xfd;
				}
				
				if (m_IsOpneQinang3) {
					buffer[4] |=  0x04;
				}
				else {
					buffer[4] &=  0xfb;
				}
				
				if (m_IsOpneQinang4) {
					buffer[4] |=  0x08;
				}
				else {
					buffer[4] &=  0xf7;
				}
			}
#endif
        }

		if (IsZhenDongFangXiangPan) {
			buffer[6] = 0x55;
		}
		else {
			if (IsOpenFangXiangPanPower) {
				buffer[6] = 0xaa;
			}
			else {
				buffer[6] = 0x00;
			}
		}

		if (IsCleanHidCoin) {
			buffer[2] = 0xaa;
			buffer[3] = 0x01;
			if (CoinCurPcvr == 0) {
				IsCleanHidCoin = false;
			}
		}
		else {
			buffer[2] = 0x00;
		}

		//FangXiangPanInfo
//		buffer[35] = FangXiangPanL_1;
//		buffer[36] = FangXiangPanL_2;
//		buffer[37] = FangXiangPanR_1;
//		buffer[38] = FangXiangPanR_2;
//		buffer[39] = FangXiangPanM_1;
//		buffer[40] = FangXiangPanM_2;

		if (IsJiaoYanHid) {
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMa[i];
			}

			for (int i = 0; i < 4; i++) {
				buffer[i + 14] = JiaoYanDt[i];
			}
		}
		else {
			RandomJiaoYanMiMaVal();
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMaRand[i];
			}

			//0x41 0x42 0x43 0x44
			for (int i = 15; i < 18; i++) {
				buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
			}
			buffer[14] = 0x00;
			
			for (int i = 15; i < 18; i++) {
				buffer[14] ^= buffer[i];
			}
		}

		buffer[5] = 0x00;
		for (int i = 2; i < 12; i++) {
			buffer[5] ^= buffer[i];
		}

		buffer[19] = 0x00;
		for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenWrite - 2); i++) {
			if (i == 19) {
				continue;
			}
			buffer[19] ^= buffer[i];
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}

	public void StartJiaoYanIO()
	{
		if (IsJiaoYanHid) {
			return;
		}

		if (!HardWareTest.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		RandomJiaoYanDt();
		IsJiaoYanHid = true;
		CancelInvoke("CloseJiaoYanIO");
		Invoke("CloseJiaoYanIO", 5f);
		//ScreenLog.Log("开始校验...");
	}

	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);

		if (HardWareTest.IsTestHardWare) {
			HardWareTest.Instance.JiaMiJiaoYanFailed();
		}
	}

	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		if (IsInvoking("CloseJiaoYanIO")) {
			CancelInvoke("CloseJiaoYanIO");
		}

		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			break;

		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardWareTest.IsTestHardWare) {
				HardWareTest.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//Debug.Log("*****JiaoYanState "+JiaoYanState);

		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//Debug.Log("JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//Debug.Log("JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	public static bool IsJiaMiJiaoYanFailed;
	
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x03;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	
	//#define First_pin			 	0xe5
	//#define Second_pin		 	0x5d
	//#define Third_pin		 		0x8c
	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0xe5; //0xff;
		JiaoYanMiMa[2] = 0x5d; //0xff;
		JiaoYanMiMa[3] = 0x8c; //0xff;
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}

	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}

		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}

		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] = JiaoYanMiMaRand[0] == 0x00 ?
				(byte)UnityEngine.Random.Range(0x01, 0xff) : (byte)(JiaoYanMiMaRand[0] + UnityEngine.Random.Range(0x01, 0xff));
		}
	}

	public static bool IsJiaoYanHid;
	public static int CountFXZD;
	public static int CountQNZD;
	public void OpenFangXiangPanZhenDong()
	{
		if (IsPlayFangXiangPanZhenDong) {
			return;
		}
		IsPlayFangXiangPanZhenDong = true;
		CountFXZD++;
		//Debug.Log("OpenFangXiangPanZhenDong -> CountFXZD "+CountFXZD+", CountQNZD "+CountQNZD);
		StartCoroutine(PlayFangXiangPanZhenDong());
	}

	public static bool IsSlowLoopCom = false;
	IEnumerator PlayFangXiangPanZhenDong()
	{
		int count = 100;
		//int count = UnityEngine.Random.Range(1, 4);
		//count = 100; //test
		do {
			IsZhenDongFangXiangPan = !IsZhenDongFangXiangPan;
			count--;
			yield return new WaitForSeconds(0.05f);
		} while (count > -1);
		IsZhenDongFangXiangPan = false;
		//IsZhenDongFangXiangPan = true; //test
		IsPlayFangXiangPanZhenDong = false;
	}

	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;

	public void GetMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}

		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}

        if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//Debug.Log("ReadBufLen was wrong! len is "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
			return;
		}

        if (IsJiOuJiaoYanFailed) {
			return;
		}

        // 协议规定 0x22 为偶数表示正常，为奇数表示 Io 板自身有问题
        if ((MyCOMDevice.ComThreadClass.ReadByteMsg[22]&0x01) == 0x01) {
			JiOuJiaoYanCount++;
			if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
				IsJiOuJiaoYanFailed = true;
				//JiOuJiaoYanFailed
			}
		}
        //IsJiOuJiaoYanFailed = true; //test

        // 全包(但跳过一些特定字节)做校验
        byte tmpVal = 0x00;
		string testA = "";
		for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
			if (i == 8 || i == 21) {
				continue;
			}
			testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		tmpVal ^= EndRead_1;
		tmpVal ^= EndRead_2;
		testA += EndRead_1 + " ";
		testA += EndRead_2 + " ";

        if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[21]) {            
            if (MyCOMDevice.ComThreadClass.IsStopComTX) {
                return;
			}            
            MyCOMDevice.ComThreadClass.IsStopComTX = true;
//			ScreenLog.Log("testA: "+testA);
//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+MyCOMDevice.ComThreadClass.ReadByteMsg[21].ToString("X2"));
//			ScreenLog.Log("byte21 was wrong!");
			return;
		}

        if (IsJiaoYanHid) {
			tmpVal = 0x00;
//			string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
//			string testStrB = "";
//			string testStrA = "";
//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("readStr: "+testStrA);

//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendDt: "+testStrB);

//			string testStrC = "";
//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendMiMa: "+testStrC);

			for (int i = 11; i < 14; i++) {
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			}

            if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[10]) {
				bool isJiaoYanDtSucceed = false;
				tmpVal = 0x00;
				for (int i = 15; i < 18; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				}
				
				//校验2...
				if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[14]
                //  校验，烦，去掉
				/*  && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[15]
				    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[16]
				    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[17] */ ) {
					isJiaoYanDtSucceed = true;
				}

				if (isJiaoYanDtSucceed) {
					//JiaMiJiaoYanSucceed
					OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
					//ScreenLog.Log("JMJYCG...");
				}
//				else {
//					testStrA = "";
//					for (int i = 0; i < 46; i++) {
//						testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//
//					string testStrC = "";
//					for (int i = 34; i < 38; i++) {
//						testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//
//					for (int i = 0; i < 4; i++) {
//						testStrC += JiaoYanDt[i].ToString("X2") + " ";
//					}
//					ScreenLog.Log("ReadByte[0 - 45] "+testStrA);
//					ScreenLog.Log("ReadByte[34 - 37] "+testStrB);
//					ScreenLog.Log("SendByte[21 - 24] "+testStrC);
//					ScreenLog.LogError("校验数据错误!");
//				}
			}
//			else {
//				testStrB = "byte[30] "+MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2")+" "
//					+", tmpVal "+tmpVal.ToString("X2");
//				ScreenLog.Log("ReadByte[30 - 33] "+testStrA);
//				ScreenLog.Log(testStrB);
//				ScreenLog.LogError("ReadByte[30] was wrong!");
//			}
		}

        //Debug.Log("GetMessage() -> 9");

        CheckIsPlayerActivePcvr();
		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		KeyProcess( readMsg );
	}

	void CheckBikeDirLen()
	{
		BikeDirLenA = SteerValMin - SteerValCen + 1;
		BikeDirLenB = SteerValCen - SteerValMax + 1;
		BikeDirLenC = SteerValMax - SteerValCen + 1;
		BikeDirLenD = SteerValCen - SteerValMin + 1;
	}

	static bool IsHandleDirByKey = true;
	public static void GetPcvrSteerVal()
	{
		if (IsHandleDirByKey) {
			//if (!bIsHardWare || IsTestGame) {
			if (!bIsHardWare) {
				mGetSteer = Input.GetAxis("Horizontal");
				return;
			}
		}
		/*else {
			if (!bIsHardWare || IsTestGame) {
				mGetSteer = Input.GetAxis("Horizontal");
				return;
			}
		}*/

		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}

		if (IsInitFangXiangJiaoZhun) {
			return;
		}

		uint bikeDir = SteerValCur;
		uint bikeDirLen = SteerValMax - SteerValMin + 1;
		if (SteerValMax < SteerValMin) {
			bikeDirLen = bikeDir > SteerValCen ? BikeDirLenA : BikeDirLenB;
			bikeDir = Math.Min(bikeDir, SteerValMin);
			bikeDir = Math.Max(bikeDir, SteerValMax);
		}
		else {
			bikeDirLen = bikeDir > SteerValCen ? BikeDirLenC : BikeDirLenD;
			bikeDir = Math.Max(bikeDir, SteerValMin);
			bikeDir = Math.Min(bikeDir, SteerValMax);
		}
		bikeDirLen = Math.Max(1, bikeDirLen);
		
		uint bikeDirCur = SteerValMax - bikeDir;
		float bikeDirPer = (float)bikeDirCur / bikeDirLen;
		if (SteerValMax > SteerValMin) {
			//ZhengJie FangXiangDianWeiQi
			if (bikeDir > SteerValCen) {
				bikeDirCur = bikeDir - SteerValCen;
				bikeDirPer = (float)bikeDirCur / bikeDirLen;
			}
			else {
				bikeDirCur = SteerValCen - bikeDir;
				bikeDirPer = - (float)bikeDirCur / bikeDirLen;
			}
		}
		else {
			//FanJie DianWeiQi
			if(bikeDir > SteerValCen) {
				bikeDirCur = bikeDir - SteerValCen;
				bikeDirPer = - (float)bikeDirCur / bikeDirLen;
			}
			else {
				bikeDirCur = SteerValCen - bikeDir;
				bikeDirPer = (float)bikeDirCur / bikeDirLen;
			}
		}
		mGetSteer = bikeDirPer;
		//Debug.Log("*** mGetSteer "+mGetSteer+", SteerValMax "+SteerValMax+", SteerValMin "+SteerValMin+", bikeDirCur "+bikeDirCur);
	}

#if OUT_INFO_TEST
	void OnGUI()
	{
		string strA = "mGetSteer "+mGetSteer.ToString("f2")
				+", SteerValCur "+SteerValCur
				+", SteerValMax "+SteerValMax
				+", SteerValMin "+SteerValMin
				+", SteerValCen "+SteerValCen;
		strA = "IsOpenFangXiangPanPower "+IsOpenFangXiangPanPower;
		GUI.Label(new Rect(10f, 200f, Screen.width, 25f), strA);
	}
#endif

	static float TimePowerLast;
	static float TimePowerMax = 3f;
	static float PowerLastVal;
	static bool IsAddSpeed;
	public static void GetPcvrPowerVal()
	{
		if (!bIsHardWare) {
		//if (!bIsHardWare || IsTestGame) {
			float valVer = Input.GetAxis("Vertical");
			float powerTmp = 0f;
			if (valVer > 0f) {
				if (!IsAddSpeed) {
					IsAddSpeed = true;
					TimePowerLast = Time.realtimeSinceStartup;
				}

				if (Time.realtimeSinceStartup - TimePowerLast < TimePowerMax) {
					powerTmp = (Time.realtimeSinceStartup - TimePowerLast) / TimePowerMax;
				}
				else {
					powerTmp = 1f;
				}
			}
			else {
				if (IsAddSpeed) {
					IsAddSpeed = false;
					PowerLastVal = mGetPower;
					TimePowerLast = Time.realtimeSinceStartup;
				}
				
				if (Time.realtimeSinceStartup - TimePowerLast < TimePowerMax && mGetPower > 0f) {
					powerTmp = (Time.realtimeSinceStartup - TimePowerLast) / TimePowerMax;
					powerTmp = PowerLastVal > powerTmp ? (PowerLastVal - powerTmp) : 0f;
				}
				else {
					powerTmp = 0f;
				}
			}
			powerTmp = powerTmp <= YouMemnMinVal ? 0f : powerTmp;
			mGetPower = powerTmp;
			mGetPower = 1f; //test.
			return;
		}

		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}

		if (IsInitYouMenJiaoZhun) {
			return;
		}

		uint bikePowerCurValTmp = 0;
		if (mBikePowerMin > mBikePowerMax) {
			bikePowerCurValTmp = Math.Min(BikePowerCur, mBikePowerMin);
			bikePowerCurValTmp = Math.Max(bikePowerCurValTmp, mBikePowerMax);
		}
		else {
			bikePowerCurValTmp = Math.Max(BikePowerCur, mBikePowerMin);
			bikePowerCurValTmp = Math.Min(bikePowerCurValTmp, mBikePowerMax);
		}
		
		uint bikePowerDis = mBikePowerMin > mBikePowerMax ? (mBikePowerMin - bikePowerCurValTmp) : (bikePowerCurValTmp - mBikePowerMin);
		float valThrottleTmp = (float)bikePowerDis / BikePowerLen;
		valThrottleTmp = valThrottleTmp <= YouMemnMinVal ? 0f : valThrottleTmp;
		valThrottleTmp = valThrottleTmp > 1f ? 1f : valThrottleTmp;
		mGetPower = valThrottleTmp;
		
//		if (IsTestGame) {
//			mGetPower = 1f; //test
//		}
	}
	public static float YouMemnMinVal = 0.1f;

	public static void GetPcvrShaCheVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}
		
		if (IsInitShaCheJiaoZhun) {
			return;
		}
		
		uint bikeShaCheCurValTmp = 0;
		if (mBikeShaCheMin > mBikeShaCheMax) {
			bikeShaCheCurValTmp = Math.Min(BikeShaCheCur, mBikeShaCheMin);
			bikeShaCheCurValTmp = Math.Max(bikeShaCheCurValTmp, mBikeShaCheMax);
		}
		else {
			bikeShaCheCurValTmp = Math.Max(BikeShaCheCur, mBikeShaCheMin);
			bikeShaCheCurValTmp = Math.Min(bikeShaCheCurValTmp, mBikeShaCheMax);
		}
		
		uint bikeShaCheDis = mBikeShaCheMin > mBikeShaCheMax ? (mBikeShaCheMin - bikeShaCheCurValTmp) : (bikeShaCheCurValTmp - mBikeShaCheMin);
		float valTmp = (float)bikeShaCheDis / BikeShaCheLen;
		valTmp = valTmp <= 0.3f ? 0f : 1f;
		if (IsTestGame) {
			return; //test
		}

//		if (!IsActiveSheCheEvent && valTmp > 0.3f) {
//			IsActiveSheCheEvent = true;
//			InputEventCtrl.GetInstance().ClickShaCheBt( ButtonState.DOWN );
//		}
//		else if (IsActiveSheCheEvent && valTmp < 0.3f){
//			IsActiveSheCheEvent = false;
//			InputEventCtrl.GetInstance().ClickShaCheBt( ButtonState.UP );
//		}
	}

	public void SubPlayerCoin(int subNum)
	{
		//Debug.Log("SubPlayerCoin ---- num "+subNum);
		if (gOldCoinNum >= subNum) {
			gOldCoinNum = (uint)(gOldCoinNum - subNum);
		}
		else {
			SubCoinNum = (int)(subNum - gOldCoinNum);
			if (mOldCoinNum < SubCoinNum) {
				return;
			}
			//Debug.Log("mOldCoinNum "+mOldCoinNum+", SubCoinNum "+SubCoinNum);
			mOldCoinNum -= (uint)SubCoinNum;
			GlobalData.CoinCur = (int)mOldCoinNum;
			gOldCoinNum = 0;
		}
	}
	
	public void InitYouMenJiaoZhun()
	{
		if (IsInitYouMenJiaoZhun) {
			return;
		}
		//ScreenLog.Log("pcvr -> InitYouMenJiaoZhun...");
		mBikePowerMin = 999999;
		mBikePowerMax = 0;
		
		IsJiaoZhunFireBt = false;
		IsInitYouMenJiaoZhun = true;
	}

	void ResetYouMenJiaoZhun()
	{
		if (!IsInitYouMenJiaoZhun) {
			return;
		}
		//ScreenLog.Log("pcvr -> ResetYouMenJiaoZhun...");
		IsJiaoZhunFireBt = false;
		IsInitYouMenJiaoZhun = false;
		bIsJiaoYanBikeValue = false;
		
		uint TmpVal = 0;
		if (IsFanZhuangYouMen) {
			TmpVal = mBikePowerMax;
			mBikePowerMax = mBikePowerMin;
			mBikePowerMin = TmpVal;
			BikePowerLen = mBikePowerMin - mBikePowerMax + 1;
			//ScreenLog.Log("YouMenFanZhuang -> mBikePowerMax = " + mBikePowerMax + ", mBikePowerMin = " + mBikePowerMin);
		}
		else {
			BikePowerLen = mBikePowerMax - mBikePowerMin + 1;
			//ScreenLog.Log("YouMenZhengZhuang -> mBikePowerMax = " + mBikePowerMax + ", mBikePowerMin = " + mBikePowerMin);
		}
		BikePowerLen = Math.Max(1, BikePowerLen);

		ReadGameInfo conf = ReadGameInfo.GetInstance();

        conf.ThrustMax = (int)mBikePowerMax;
		conf.ThrustMin = (int)mBikePowerMin;
	}

	public void InitShaCheJiaoZhun()
	{
		if (IsInitShaCheJiaoZhun) {
			return;
		}
		mBikeShaCheMin = 999999;
		mBikeShaCheMax = 0;
		IsJiaoZhunFireBt = false;
		IsInitShaCheJiaoZhun = true;
	}

	void ResetShaCheJiaoZhun()
	{
		if (!IsInitShaCheJiaoZhun) {
			return;
		}
		IsJiaoZhunFireBt = false;
		IsInitShaCheJiaoZhun = false;
		bIsJiaoYanBikeValue = false;
		
		uint TmpVal = 0;
		if (IsFanZhuangShaChe) {
			TmpVal = mBikeShaCheMax;
			mBikeShaCheMax = mBikeShaCheMin;
			mBikeShaCheMin = TmpVal;
			BikeShaCheLen = mBikeShaCheMin - mBikeShaCheMax + 1;
		}
		else {
			BikeShaCheLen = mBikeShaCheMax - mBikeShaCheMin + 1;
		}
		BikeShaCheLen = Math.Max(1, BikeShaCheLen);

        ReadGameInfo conf = ReadGameInfo.GetInstance();
        conf.BrakeMax = (int)mBikeShaCheMax;
		conf.BrakeMin = (int)mBikeShaCheMin;
	}

	public void InitFangXiangJiaoZhun()
	{
		if (IsInitFangXiangJiaoZhun) {
			return;
		}
		//ScreenLog.Log("pcvr -> InitFangXiangJiaoZhun...");
		//FangXiangInfo
		SteerValMin = 999999;
		SteerValCen = 1765;
		SteerValMax = 0;
		
		IsJiaoZhunFireBt = false;
		FangXiangJiaoZhunCount = 0;
		IsInitFangXiangJiaoZhun = true;
		bIsJiaoYanBikeValue = true;
	}
	
	void ResetFangXiangJiaoZhun()
	{
		if (!IsInitFangXiangJiaoZhun) {
			return;
		}
		//ScreenLog.Log("pcvr -> ResetFangXiangJiaoZhun...");
		IsJiaoZhunFireBt = false;
		FangXiangJiaoZhunCount = 0;
		IsInitFangXiangJiaoZhun = false;
		
		uint TmpVal = 0;
		if (IsFanZhuangFangXiang) {
			TmpVal = SteerValMax;
			SteerValMax = SteerValMin;
			SteerValMin = TmpVal;
			//ScreenLog.Log("CheTouFangXiangFanZhuan -> SteerValMin " + SteerValMin + ", SteerValMax " +SteerValMax);
		}
		else {
			//ScreenLog.Log("CheTouFangXiangZhengZhuan -> SteerValMin " + SteerValMin + ", SteerValMax " +SteerValMax);
		}
		CheckBikeDirLen();

        ReadGameInfo conf = ReadGameInfo.GetInstance();
        conf.SteerMin = (int)SteerValMin;
		conf.SteerCenter = (int)SteerValCen;
		conf.SteerMax = (int)SteerValMax;
	}

	void ShaCheJiaoZhun()
	{
        ReadGameInfo conf = ReadGameInfo.GetInstance();

        if (!IsInitShaCheJiaoZhun) {
			return;
		}
		
		if (BikeShaCheCur < mBikeShaCheMin) {
			mBikeShaCheMin = BikeShaCheCur;
			conf.BrakeMin = (int)mBikeShaCheMin;
		}
		
		if (BikeShaCheCur > mBikeShaCheMax) {
			mBikeShaCheMax = BikeShaCheCur;
            conf.BrakeMax = (int)mBikeShaCheMax;
        }
		
		if (bPlayerStartKeyDown && !IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = true;
			uint dVal_0 = BikeShaCheCur - mBikeShaCheMin;
			uint dVal_1 = mBikeShaCheMax - BikeShaCheCur;
			if (dVal_0 > dVal_1) {
				IsFanZhuangShaChe = false;
			}
			else if (dVal_0 < dVal_1) {
				IsFanZhuangShaChe = true;
			}
			ResetShaCheJiaoZhun();
		}
		else if(!bPlayerStartKeyDown && IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = false;
		}
	}

	void YouMenJiaoZhun()
	{
        if (!IsInitYouMenJiaoZhun) {
			return;
		}

		if (BikePowerCur < mBikePowerMin) {
			mBikePowerMin = BikePowerCur;
        }
		
		if (BikePowerCur > mBikePowerMax) {
			mBikePowerMax = BikePowerCur;
        }
		
		if (bPlayerStartKeyDown && !IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = true;
			uint dVal_0 = BikePowerCur - mBikePowerMin;
			uint dVal_1 = mBikePowerMax - BikePowerCur;
			if (dVal_0 > dVal_1) {
				//YouMenZhengZhuang
				IsFanZhuangYouMen = false;
			}
			else if (dVal_0 < dVal_1) {
				//YouMenFanZhuang
				IsFanZhuangYouMen = true;
			}
			ResetYouMenJiaoZhun();
			//InitShaCheJiaoZhun();
			IsJiaoZhunFireBt = true;
		}
		else if(!bPlayerStartKeyDown && IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = false;
		}
	}

	void FangXiangJiaoZhun()
	{
		if (!IsInitFangXiangJiaoZhun) {
			return;
		}

        ReadGameInfo conf = ReadGameInfo.GetInstance();
        //Record FangXiangInfo
        if (SteerValCur < SteerValMin) {
			SteerValMin = SteerValCur;
			conf.SteerMin = (int)SteerValMin;
		}
		
		if (SteerValCur > SteerValMax) {
			SteerValMax = SteerValCur;
            conf.SteerMax = (int)SteerValMax;
		}
		
		if (bPlayerStartKeyDown && !IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = true;
			FangXiangJiaoZhunCount++;
			switch (FangXiangJiaoZhunCount) {
			case 1:
				//CheTouZuoZhuan
				uint dVal_0 = SteerValCur - SteerValMin;
				uint dVal_1 = SteerValMax - SteerValCur;
				if (dVal_0 < dVal_1) {
					IsFanZhuangFangXiang = false;
				}
				else if (dVal_0 > dVal_1) {
					IsFanZhuangFangXiang = true;
				}
				break;
				
			case 2:
				//CheTouZhuanDaoZhongJian
				SteerValCen = SteerValCur;
				break;
				
			case 3:
				//CheTouYouZhuan
				ResetFangXiangJiaoZhun();
				InitYouMenJiaoZhun();
				IsJiaoZhunFireBt = true;
				break;
			}
		}
		else if(!bPlayerStartKeyDown && IsJiaoZhunFireBt) {
			IsJiaoZhunFireBt = false;
		}
	}

	public static uint BikeBeiYongPowerCurPcvr;
	void KeyProcess(uint []buffer)
	{
        //Debug.Log("KeyProcess() -> 1");
        if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}

        //Debug.Log("KeyProcess() -> 2");

        if (buffer[0] != ReadHead_1 || buffer[1] != ReadHead_2) {
			return;
		}

        //Debug.Log("KeyProcess() -> 3");
        SteerValCur = ((buffer[6]&0x0f) << 8) + buffer[7]; //fangXiang
		bool isTest = false;
		if (!isTest) {
//			if (IsTestGame) {
//				BikePowerCur = ((buffer[6]&0x0f) << 8) + buffer[7]; //用方向信息测试油门.
//			}
//			else {
//				BikePowerCur = ((buffer[2]&0x0f) << 8) + buffer[3]; //youMen
//			}
			BikePowerCur = ((buffer[2]&0x0f) << 8) + buffer[3]; //youMen
			BikePowerCurPcvr = BikePowerCur;
			
			BikeShaCheCur = ((buffer[4]&0x0f) << 8) + buffer[5]; //shaChe
			ShaCheCurPcvr = BikeShaCheCur;
		}
		else {
			BikePowerCur = SteerValCur; //test
			BikeShaCheCur = SteerValCur; //test
		}

		if (HardWareTest.IsTestHardWare) {
			uint tmpBYYouMen = ((buffer[2]&0x0f) << 8) + buffer[3]; //youMen
			BikeBeiYongPowerCurPcvr = tmpBYYouMen;
		}

        //		if (!IsInitYouMenJiaoZhun) {
        //			float dPower = BikePowerOld > BikePowerCur ? BikePowerOld - BikePowerCur : BikePowerCur - BikePowerOld;
        //			if (mBikePowerMax > mBikePowerMin) {
        //				if (dPower / (mBikePowerMax - mBikePowerMin) > 0.3f) {
        //					BikePowerCur = mBikePowerMin;
        //				}
        //			}
        //			else {
        //				if (dPower / (mBikePowerMin - mBikePowerMax) > 0.3f) {
        //					BikePowerCur = mBikePowerMax;
        //				}
        //			}
        //			BikePowerOld = BikePowerCur;
        //		}

        //Debug.Log("KeyProcess() -> 4");
        //game coinInfo
        CoinCurPcvr = buffer[8];
		if (CoinCurPcvr > 0) {
			if (!IsCleanHidCoin) {
				IsCleanHidCoin = true;
				mOldCoinNum += CoinCurPcvr;
				GlobalData.CoinCur = (int)mOldCoinNum;
				//ScreenLog.Log("player insert coin, game coinNum: " + mOldCoinNum );
			}
		}

        //Debug.Log("KeyProcess() -> 5");
        if (bIsJiaoYanBikeValue) {
			FangXiangJiaoZhun();
			YouMenJiaoZhun();
			ShaCheJiaoZhun();
		}

        //Debug.Log("KeyProcess() -> buffer[9] : " + buffer[9] + ", IsCloseDongGanBtDown : " + IsCloseDongGanBtDown);
        if ( !IsCloseDongGanBtDown && 0x02 == (buffer[9]&0x02) ) {
            // ScreenLog.Log("game DongGanBt down!");
            //Debug.Log("game DongGanBt down!");
            IsCloseDongGanBtDown = true;
			InputEventCtrl.GetInstance().ClickCloseDongGanBt( ButtonState.DOWN );
		}
		else if ( IsCloseDongGanBtDown && 0x00 == (buffer[9]&0x02) ) {
            // ScreenLog.Log("game DongGanBt up!");
            //Debug.Log("game DongGanBt up!");
            IsCloseDongGanBtDown = false;
			InputEventCtrl.GetInstance().ClickCloseDongGanBt( ButtonState.UP );
		}
		
		if (pcvr.bIsHardWare && !pcvr.IsTestGame) {
			//Debug.Log("KeyProcess() -> 6");
			//if ( !bPlayerStartKeyDown && 0x01 == (buffer[28]&0x01) ) { //test
			if ( !bPlayerStartKeyDown && 0x01 == (buffer[9]&0x01) ) {
				//ScreenLog.Log("game startBt down!");
				//Debug.Log("game startBt down!");
				bPlayerStartKeyDown = true;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
			}
			//else if ( bPlayerStartKeyDown && 0x00 == (buffer[28]&0x01) ) { //test
			else if ( bPlayerStartKeyDown && 0x00 == (buffer[9]&0x01) ) {
				//ScreenLog.Log("game startBt up!");
				//Debug.Log("game startBt up!");
				bPlayerStartKeyDown = false;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
			}
		}
		
		//Debug.Log("KeyProcess() -> 7");
		if ( !bSetEnterKeyDown && 0x10 == (buffer[9]&0x10) ) {
			bSetEnterKeyDown = true;
            //ScreenLog.Log("game setEnterBt down!");
            //Debug.Log("game setEnterBt down!");
            InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
		}
		else if ( bSetEnterKeyDown && 0x00 == (buffer[9]&0x10) ) {
			bSetEnterKeyDown = false;
			//ScreenLog.Log("game setEnterBt up!");
            //Debug.Log("game setEnterBt up!");
            InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
		}

        //Debug.Log("KeyProcess() -> 8");
        if ( !bSetMoveKeyDown && 0x20 == (buffer[9]&0x20) ) {
			bSetMoveKeyDown = true;
            // ScreenLog.Log("game setMoveBt down!");
            //Debug.Log("game setMoveBt down!");
            InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
		}
		else if( bSetMoveKeyDown && 0x00 == (buffer[9]&0x20) ) {
			bSetMoveKeyDown = false;
            // ScreenLog.Log("game setMoveBt up!");
            //Debug.Log("game setMoveBt up!");
            InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
		}

        //Debug.Log("KeyProcess() -> 9");

        if ( !IsClickLaBaBt && 0x04 == (buffer[9]&0x04) ) {
			IsClickLaBaBt = true;
            //			ScreenLog.Log("game LaBaBt down!");
            //Debug.Log("game LaBaBt down!");
            InputEventCtrl.GetInstance().ClickLaBaBt( ButtonState.DOWN );
		}
		else if( IsClickLaBaBt && 0x00 == (buffer[9]&0x04) ) {
			IsClickLaBaBt = false;
            // ScreenLog.Log("game LaBaBt up!");
            //Debug.Log("game LaBaBt up!");
            InputEventCtrl.GetInstance().ClickLaBaBt( ButtonState.UP );
		}
    }

	public static bool IsPlayerActivePcvr = true;
	public static float TimeLastActivePcvr;
	void CheckIsPlayerActivePcvr()
	{
		if (Application.loadedLevel == 1) {
			return;
		}

		if (!IsPlayerActivePcvr) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f) {
			IsPlayerActivePcvr = false;
		}
	}
	
	public static void SetIsPlayerActivePcvr()
	{
		if (!bIsHardWare) {
			return;
		}
		IsPlayerActivePcvr = true;
		TimeLastActivePcvr = Time.realtimeSinceStartup;
	}
}

public enum StartLightState
{
	Liang,
	Shan,
	Mie
}