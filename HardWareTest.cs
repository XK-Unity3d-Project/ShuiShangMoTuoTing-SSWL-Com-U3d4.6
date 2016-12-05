using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public class HardWareTest : MonoBehaviour 
{
	public UILabel[] m_Label;
	public UILabel m_HitTimmerSet;
	public UISlider m_HitTimmerValue;
	public UILabel m_FallTimmerSet;
	public UISlider m_FallTimmerValue;

	public UILabel TouBiLabel;
	public UILabel ShaCheLabel;
	public UILabel AnJianLabel;
	public UILabel YouMenLabel;
	public UILabel FangXiangLabel;
	public static bool IsTestHardWare;
	public static HardWareTest Instance;
	void Start ()
	{
		Instance = this;
		JiaMiJiaoYanCtrlObj.SetActive(IsJiaMiTest);
		IsTestHardWare = true;
		AnJianLabel.text = "";
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
		InputEventCtrl.GetInstance().ClickCloseDongGanBtEvent += ClickCloseDongGanBtEvent;
		InputEventCtrl.GetInstance().ClickLaBaBtEvent += ClickLaBaBtEvent;
		pcvr.GetInstance();
		pcvr.CloseFangXiangPanPower();
	}
	public UILabel BeiYongYouMenLabel;
	void Update () 
	{
		TouBiLabel.text = GlobalData.CoinCur.ToString();
		ShaCheLabel.text = pcvr.ShaCheCurPcvr.ToString();
		YouMenLabel.text = pcvr.BikePowerCurPcvr.ToString();
		BeiYongYouMenLabel.text = pcvr.BikeBeiYongPowerCurPcvr.ToString();
		FangXiangLabel.text = pcvr.SteerValCur.ToString();

		m_HitTimmerSet.text = ((float)(Convert.ToDouble(m_HitTimmerValue.value)*5.0f)).ToString();
		m_FallTimmerSet.text = ((float)(Convert.ToDouble(m_FallTimmerValue.value)*5.0f)).ToString();
		m_HitshakeTimmerSet = (float)(Convert.ToDouble(m_HitTimmerSet.text));
		OnShakeHit();
	}
	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLabel.text = "SetEnter Down";
		}
		else {
			AnJianLabel.text = "SetEnter Up";
		}
	}
	void ClickLaBaBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLabel.text = "SpeakerBtDown";
		}
		else {
			AnJianLabel.text = "SpeakerBtUp";
		}
	}
	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLabel.text = "SetMove Down";
		}
		else {
			AnJianLabel.text = "SetMove Up";
		}
	}
	void ClickStartBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLabel.text = "StartBt Down";
		}
		else {
			AnJianLabel.text = "StartBt Up";
		}
	}
	void ClickCloseDongGanBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLabel.text = "DongGanBt Down";
		}
		else {
			AnJianLabel.text = "DongGanBt Up";
		}
	}
	public void OnClickForwardBt()
	{
		if(pcvr.m_IsOpneQinang3)
		{
			pcvr.m_IsOpneQinang3 = false;
			m_Label[0].text = "OffQN3";
		}
		else
		{
			pcvr.m_IsOpneQinang3 = true;
			m_Label[0].text = "OpenQN3";
		}
	}
	public void OnClickBehindBt()
	{
		if(pcvr.m_IsOpneQinang4)
		{
			pcvr.m_IsOpneQinang4 = false;
			m_Label[1].text = "OffQN4";
		}
		else
		{
			pcvr.m_IsOpneQinang4 = true;
			m_Label[1].text = "OpenQN4";
		}
	}
	public void OnClickLeftBt()
	{
		if(pcvr.m_IsOpneQinang1)
		{
			pcvr.m_IsOpneQinang1 = false;
			m_Label[2].text = "OffQN1";
		}
		else
		{
			pcvr.m_IsOpneQinang1 = true;
			m_Label[2].text = "OpenQN1";
		}
	}
	public void OnClickRightBt()
	{
		if(pcvr.m_IsOpneQinang2)
		{
			pcvr.m_IsOpneQinang2 = false;
			m_Label[3].text = "OffQN2";
		}
		else
		{
			pcvr.m_IsOpneQinang2 = true;
			m_Label[3].text = "OpenQN2";
		}
	}
	public void OnClickSubCoinBt()
	{
		pcvr.GetInstance().SubPlayerCoin(1);
	}
	public UILabel ShaCheDengLabel;
	int ShaCheCount;
	public void OnClickShaCheLightBt()
	{
		ShaCheCount++;
		switch (ShaCheCount) {
		case 0:
			ShaCheDengLabel.text = "刹车灯灭";
			pcvr.ShaCheBtLight = StartLightState.Mie;
			break;

		case 1:
			ShaCheDengLabel.text = "刹车灯半亮";
			pcvr.ShaCheBtLight = StartLightState.Shan;
			break;
			
		case 2:
			ShaCheDengLabel.text = "刹车灯全亮";
			pcvr.ShaCheBtLight = StartLightState.Liang;
			ShaCheCount = -1;
			break;
		}
	}
	public void OnClickCloseAppBt()
	{
		Application.Quit();
	}
	
	public bool IsJiaMiTest = false;
	public GameObject JiaMiJiaoYanCtrlObj;
	public void OnClickRestartAppBt()
	{
		Application.Quit();
		RunCmd("start ComTest.exe");
	}
	void RunCmd(string command)
	{
		//實例一個Process類，啟動一個獨立進程    
		Process p = new Process();    //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，    
		//包括了一些屬性和方法，下面我們用到了他的幾個屬性：   
		p.StartInfo.FileName = "cmd.exe";           //設定程序名   
		p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數   
		p.StartInfo.UseShellExecute = false;        //關閉Shell的使用    p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入    p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出   
		p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出    
		p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口    
		p.Start();   //啟動
		
		//p.WaitForInputIdle();
		//MoveWindow(p.MainWindowHandle, 1000, 10, 300, 200, true);
		
		//p.StandardInput.WriteLine(command); //也可以用這種方式輸入要執行的命令    
		//p.StandardInput.WriteLine("exit");        //不過要記得加上Exit要不然下一行程式執行的時候會當機    return p.StandardOutput.ReadToEnd();        //從輸出流取得命令執行結果
	}
	public float m_HitshakeTimmerSet = 1.0f;
	private float m_HitshakeTimmer = 0.0f;
	public static bool m_IsHitshake = false;
	public void OnHitShake()
	{
		pcvr.GetInstance().OpenFangXiangPanZhenDong();
		if (m_IsHitshake) {
			return;
		}
		//UnityEngine.Debug.Log("OnHitShake...");
		m_IsHitshake = true;
	}
	void TestLoopOpenFangXiangPanZhenDong()
	{
//		UnityEngine.Debug.Log("TestLoopOpenFangXiangPanZhenDong...");
		pcvr.GetInstance().OpenFangXiangPanZhenDong();
	}
	void OnShakeHit()
	{
		if(m_IsHitshake)
		{
			if(m_HitshakeTimmer<m_HitshakeTimmerSet)
			{
				m_HitshakeTimmer+=Time.deltaTime;
				if(m_HitshakeTimmer<m_HitshakeTimmerSet*0.25f || (m_HitshakeTimmer>=m_HitshakeTimmerSet*0.5f && m_HitshakeTimmer<m_HitshakeTimmerSet*0.75f))
				{
					pcvr.m_IsOpneForwardQinang = false;
					pcvr.m_IsOpneBehindQinang = false;
					pcvr.m_IsOpneLeftQinang = false;
					pcvr.m_IsOpneRightQinang = true;
				}
				else if((m_HitshakeTimmer>=m_HitshakeTimmerSet*0.25f &&m_HitshakeTimmer<m_HitshakeTimmerSet*0.5f) || m_HitshakeTimmer>=m_HitshakeTimmerSet*0.75f)
				{
					pcvr.m_IsOpneForwardQinang = false;
					pcvr.m_IsOpneBehindQinang = false;
					pcvr.m_IsOpneLeftQinang = true;
					pcvr.m_IsOpneRightQinang = false;
				}
			}
			else
			{
				m_HitshakeTimmer = 0.0f;
				m_IsHitshake = false;
				pcvr.m_IsOpneForwardQinang = false;
				pcvr.m_IsOpneBehindQinang = false;
				pcvr.m_IsOpneLeftQinang = false;
				pcvr.m_IsOpneRightQinang = false;
			}
		}
	}

	public UILabel StartLightLabel;
	int LightStart = 1;
	public void OnClickStartLightBt()
	{
		LightStart++;
		//Debug.Log("**************LightStart "+LightStart);
		switch (LightStart) {
		case 1:
			StartLightLabel.text = "开始灯亮";
			pcvr.StartBtLight = StartLightState.Liang;
			break;

		case 2:
			StartLightLabel.text = "开始灯闪";
			pcvr.StartBtLight = StartLightState.Shan;
			break;

		case 3:
			StartLightLabel.text = "开始灯灭";
			pcvr.StartBtLight = StartLightState.Mie;
			LightStart = 1;
			break;
		}
	}

	public UILabel FangXiangPanPowerLabel;
	public void OnClickFangXiangPanPowerBt()
	{
		switch (FangXiangPanPowerLabel.text) {
		case "方向盘力关闭":
			FangXiangPanPowerLabel.text = "方向盘力打开";
			pcvr.OpenFangXiangPanPower();
			CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
			InvokeRepeating("TestLoopOpenFangXiangPanZhenDong", 0f, 0.2f);
			break;

		case "方向盘力打开":
			FangXiangPanPowerLabel.text = "方向盘力关闭";
			pcvr.CloseFangXiangPanPower();
			CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
			break;
		}
	}

	public UILabel DongGanLightLabel;
	int LightDongGan = 1;
	public void OnClickDongGanLightBt()
	{
		LightDongGan++;
		//Debug.Log("**************LightDongGan "+LightDongGan);
		switch (LightDongGan) {
		case 1:
			DongGanLightLabel.text = "动感灯亮";
			pcvr.DongGanBtLight = StartLightState.Liang;
			break;
			
		case 2:
			DongGanLightLabel.text = "动感灯闪";
			pcvr.DongGanBtLight = StartLightState.Shan;
			break;
			
		case 3:
			DongGanLightLabel.text = "动感灯灭";
			pcvr.DongGanBtLight = StartLightState.Mie;
			LightDongGan = 1;
			break;
		}
	}
	
	public UILabel JiaMiJYLabel;
	public UILabel JiaMiJYMsg;
	public static bool IsOpenJiaMiJiaoYan;
	void CloseJiaMiJiaoYanFailed()
	{
		if (!IsInvoking("JiaMiJiaoYanFailed")) {
			return;
		}
		CancelInvoke("JiaMiJiaoYanFailed");
	}

	public void OnClickJiaMiJiaoYanBt()
	{
		if (JiaMiJYLabel.text != "开启校验" && !pcvr.IsJiaoYanHid) {
			UnityEngine.Debug.Log("OnClickJiaMiJiaoYanBt...");
			OpenJiaMiJiaoYan();
			JiaMiJYLabel.text = "开启校验";
			SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
	}
	
	public static void OpenJiaMiJiaoYan()
	{
		if (IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = true;
		//Instance.DelayCloseJiaMiJiaoYan();

		pcvr.GetInstance().StartJiaoYanIO();
	}
	
	public void DelayCloseJiaMiJiaoYan()
	{
		CloseJiaMiJiaoYanFailed();
		Invoke("JiaMiJiaoYanFailed", 5f);
	}
	
	public void JiaMiJiaoYanFailed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Failed);
	}

	public void JiaMiJiaoYanSucceed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Succeed);
	}
	
	public static void CloseJiaMiJiaoYan()
	{
		if (!IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = false;
	}
	
	void ResetJiaMiJYLabelInfo()
	{
		CloseJiaMiJiaoYan();
		JiaMiJYLabel.text = "加密校验";
	}
	
	public void SetJiaMiJYMsg(string msg, JiaMiJiaoYanEnum key)
	{
		switch (key) {
		case JiaMiJiaoYanEnum.Succeed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验成功";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验成功");
			break;
			
		case JiaMiJiaoYanEnum.Failed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验失败";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验失败");
			break;
			
		default:
			JiaMiJYMsg.text = msg;
			ScreenLog.Log(msg);
			break;
		}
	}

}

public enum JiaMiJiaoYanEnum
{
	Null,
	Succeed,
	Failed,
}