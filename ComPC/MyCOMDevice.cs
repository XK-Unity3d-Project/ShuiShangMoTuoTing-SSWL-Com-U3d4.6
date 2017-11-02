using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;
using System.Text;
using System.Runtime.InteropServices;

public static class gkioutil {
    public static void output_array(String notes, byte[] arr)
    {
        String DbgMsg = notes + " - " + arr.Length + " : ";

        for (int i = 0; i < arr.Length; i++) {
            DbgMsg += String.Format("{0:X2} ", arr[i]);
        }

        DbgMsg += " --- ";

        Debug.Log(DbgMsg);
    }
}

public class GkioPort
{
    [DllImport("gkio", EntryPoint = "gkio_open", CallingConvention = CallingConvention.Cdecl)]
    public static extern int _gkio_open();

    // 经实测，_gkio_write 写入命令，然后 _gkio_read 读取返回数据，Win7 下，
    // 运行   1000 次，耗时  2013 毫秒
    // 运行 1,0000 次，耗时 20139 / 20155 /20109 毫秒
    [DllImport("gkio", EntryPoint = "gkio_read", CallingConvention = CallingConvention.Cdecl)]
    public static extern int _gkio_read(byte[] read_buf, int read_buf_size);

    [DllImport("gkio", EntryPoint = "gkio_write", CallingConvention = CallingConvention.Cdecl)]
    public static extern int _gkio_write(byte[] write_buf, int write_buf_size);

    [DllImport("gkio", EntryPoint = "gkio_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void _gkio_close();

    public GkioPort() {
        IsOpen_ = false;
        //Debug.Log("GkioPort() created.");
    }

    ~GkioPort()
    {
        //Debug.Log("GkioPort() destroyed.");
    }

    private bool IsOpen_ = false;
    public bool IsOpen { get { return IsOpen_; } }

    static public int gkio_write_read(byte[] write_buf, byte[] read_buf)
    {
        int ret;

        ret = _gkio_write(write_buf, write_buf.Length);
        if (ret > 0) {
            ret = _gkio_read(read_buf, read_buf.Length);
        }

        return ret;
    }

    public void Open()
    {
        // 返回找到的 GKIO 板数量。目前没找到返回 0，找到 1 个或多个，都返回 1。只支持使用第一块板
        int num_of_gkio = _gkio_open();

        IsOpen_ = (num_of_gkio != 0);

#if UNITY_EDITOR
        Debug.Log("Open() called and num_of_gkio is : " + num_of_gkio);
#endif
    }

    public void Close()
    {
        if (IsOpen_) {
            _gkio_close();
        }
    }

//    private static byte[] wbuf = new byte[16];
    private static byte[] rbuf = new byte[16];

    // 扣币
    private void SubCoin(byte numToSub)
    {
        if (numToSub <= 0) {
            return;
        }

        byte[] CMD_SUB_COIN = {  // 减币指令，币数待填写
            0x61, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        CMD_SUB_COIN[3] = numToSub;

        // 返回的 rbuf[0] 应该是 0x82
        gkio_write_read(CMD_SUB_COIN, rbuf);
    }

    /* 设置路面情况。经常不灵，所以不用
    const byte ROAD_SMOOTH = 0;
    const byte ROAD_SAND = 1;
    public void SetRoad(byte roadType, Int16 power) {
        // 路面设定，让摩托艇方向盘始终有一个最基本的轻微振动感
        byte[] CMD_SET_ROAD = {  // 路面设置指令
            0x22,                // 包头
            CHANNEL_ROAD,        // 特效通道，默认 2，范围 0 到 9
            0x00,                // 路面类型，1，沙路。默认 5，坑洼路面。0 为平滑路面
            0x03, 0xe8,          // 特效力道，2 个字节范围  00 00 到 03 E8
            0x00, 0x00, 0x00,    // 不知道，协议说明上指定为 0
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };

        CMD_SET_ROAD[2] = roadType;
        CMD_SET_ROAD[3] = (byte)(power >> 8);
        CMD_SET_ROAD[4] = (byte)((power << 8) >> 8);

        // 返回的 rbuf[0] 应该是 0x22
        int ret = gkio_write_read(CMD_SET_ROAD, rbuf);
    }
    */

    private byte SteerForce_ = 75;

    // 初始化方向盘的状态，回中位置，振动力量等。一般方向盘的位置在 0xAA 附近
    // 正常情况下，设置方向盘的初始参数，这个初始化动作，应该在 Open() 打开端口
    // 以后，做一次就行了。但是那样会出错，所有后续读 gkio 板都返回全 0
    // 所以只能在每一次 write 到 gkio 的时候反复调用 InitWheel。
    // 通过 wheelInited 标记来保证只执行一次，但还是不灵，IO 经常会出错，返回 -5
    public void InitWheel(uint knuWheelCenter, byte SteerForce)
    {
        SteerForce_ = SteerForce;

        // 回中的意思是回到 "方向盘左边界" 和 "方向盘右边界" 指定的一个小区域
        byte[] CMD_SET_WHEEL = {  // 方向盘力反馈指令
            0x21,        // 包头
            0x00,        // 方向盘左边界
            0x00,        // 方向盘右边界
            0x00, 0x00,  // 方向盘力道
            0x00,        // 方向盘类型
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        // Knu Io 板上 STM32 芯片的 ADC 硬件是 12-bit 测量结果不可能大于 0xFFE
        const int ADC_UPPER_LIMIT = 0xFFE;
        if (knuWheelCenter > ADC_UPPER_LIMIT) {
            knuWheelCenter = ADC_UPPER_LIMIT;
        }

        // knuIo 板的方向盘中间位置，是 12 bit 的，Gkio 板只支持 8 bit 数据
        // 所以舍去低 4 位 Gkio 才能理解。程序关心的是比例，绝对数值不重要
        byte GkioCenter = (byte)(knuWheelCenter >> 4);
        // 方向盘作用边界
        CMD_SET_WHEEL[1] = (byte)(GkioCenter - 1);
        CMD_SET_WHEEL[2] = GkioCenter;

        // 特定模式下，还可以在 3 和 4 字节继续细分方向盘力道, 范围 0 到 0x0388
        CMD_SET_WHEEL[3] = 0x02;
        CMD_SET_WHEEL[4] = 0x64;

        // 设置为正常模式，力道强
        CMD_SET_WHEEL[5] = 0x02;

        // 返回的 rbuf[0] 应该是 0x21
        //int ret = gkio_write_read(CMD_SET_WHEEL, rbuf);
        gkio_write_read(CMD_SET_WHEEL, rbuf);
    }

    // force 在 0 到 100 之间
    private static void setMoterPower(byte force)
    {
        byte[] CMD_WHEEL_FORCE = {  // 方向盘电机功率。0 为关闭，0x64 为全功率
            0x28,   // 包头
            0x00,   // 电机力量，0 为关闭，最大 0x64
            0x64,   // 总是 0x64
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };

        if (force > 100) {
            force = 100;
        }

        CMD_WHEEL_FORCE[1] = force;

        // 返回的 rbuf[0] 应该是 0x28
        gkio_write_read(CMD_WHEEL_FORCE, rbuf);
    }

    // 特效通道，不同的效果要用不同的通道，才能叠加
    const byte CHANNEL_SHAKE_WHEEL = 02;
    const byte CHANNEL_ROAD = 08;

    private static void shakeWheel()
    {
        byte[] CMD_SHAKE_WHEEL = { // 这个振动感觉稍好
            0x23,
            CHANNEL_SHAKE_WHEEL,
            0x00, 0x00, 0x05, 0x03, 0xE8, 0x0A,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        // 返回的 rbuf[0] 应该是 0x28
        gkio_write_read(CMD_SHAKE_WHEEL, rbuf);
    }

    // Gkio 有 0 - 39 共 40 组输出，所以 idx 要在这个范围内
    private static void turnOutputOn(byte idx)
    {
        byte[] CMD_OUTPUT_ON = {
            0x61, 0x81, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        CMD_OUTPUT_ON[2] = idx;
        CMD_OUTPUT_ON[3] = 0xff;

        // 返回的 rbuf[0] 应该是 0x81
        gkio_write_read(CMD_OUTPUT_ON, rbuf);
    }

    private static void turnOutputOff(byte idx)
    {
        byte[] CMD_OUTPUT_OFF = {
            0x61, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        CMD_OUTPUT_OFF[2] = idx;
        CMD_OUTPUT_OFF[3] = 0x00;

        // 返回的 rbuf[0] 应该是 0x81
        gkio_write_read(CMD_OUTPUT_OFF, rbuf);
    }

    // 开始灯
    private static void setGascell(byte gascell)
    {
        bool gascell_1 = ((gascell & 0x01) == 0x01);
        if (gascell_1) {
            turnOutputOn(18);
        }
        else {
            turnOutputOff(18);
        }

        bool gascell_2 = ((gascell & 0x02) == 0x02);
        if (gascell_2) {
            turnOutputOn(19);
        }
        else {
            turnOutputOff(19);
        }

        bool gascell_3 = ((gascell & 0x04) == 0x04);
        if (gascell_3) {
            turnOutputOn(20);
        }
        else {
            turnOutputOff(20);
        }

        bool gascell_4 = ((gascell & 0x08) == 0x08);
        if (gascell_4) {
            turnOutputOn(21);
        }
        else {
            turnOutputOff(21);
        }
    }

    // 输出的强度，范围从 0 到 60000。推动气囊控制板的继电器和 LED 灯
    private void initOutputPower()
    {
        byte[] CMD_OUTPUT_POWER = {
            0x61, 0x91, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        byte[] wbuf = new byte[16];
        byte[] rbuf = new byte[16];

        Array.Copy(CMD_OUTPUT_POWER, wbuf, 16);

        // 气囊接在 18 19 20 21 号输出，LED 灯接在 22 号输出
        for (byte idx = 18; idx <= 22; ++idx) {
            wbuf[2] = idx;

            // 亮度 50,000
            wbuf[3] = 0xC3;
            wbuf[4] = 0x50;

            gkio_write_read(wbuf, rbuf);
        }
    }

    // Gkio 板有 0 到 39 共 40 个输出。我们把气囊接在 18 19 20 21 号输出，LED 灯接在 22 号输出
    private void setLight(byte light)
    {
        const byte LED_LIGHT_IDX = 22;
        switch (light) {
            case 0x00:
                turnOutputOff(LED_LIGHT_IDX);
                break;
            case 0x55:
                turnOutputOn(LED_LIGHT_IDX);
                break;
            case 0xaa:
                turnOutputOn(LED_LIGHT_IDX);
                break;
        }
    }

    // KnuIo 板数据包
    public void Write(byte[] knuPkt, int offset, int count)
    {
        // knuPkt[2 - 3] 是减币
        if (knuPkt[2] == 0xaa) {
            SubCoin(knuPkt[3]);
        }
		
		int steerForce = ReadGameInfo.SteerForceVal;
		InitWheel(pcvr.SteerValCen, (byte)steerForce);
//		ReadGameInfo conf = ReadGameInfo.GetInstance();
//      InitWheel(pcvr.SteerValCen, (byte)conf.SteerForce);

        // knuPkt[4] 是气囊和开始灯
        setGascell(knuPkt[4]);

        // knuPkt[6] 是方向盘振动和力
        const byte MOTOR_POWER_OFF = 0;
        //const byte MOTOR_FULL_POWER = 65;  // 玩家反映设为 100 满载时力太大

        //Debug.Log(String.Format("Write() called. The wheel power byte is : {0:X2}", knuPkt[6]));

        switch (knuPkt[6]) {
            case 0x00:  // Demo 状态下，关闭电机电源，方向盘不自动回中
                setMoterPower(MOTOR_POWER_OFF);
                break;
            case 0x55:  // 振动一次
                shakeWheel();
                break;            
            case 0xaa:  // 开始游戏，打开电机电源，方向盘自动回中
                setMoterPower(SteerForce_);
                break;
            default:    // 到这里说明出错，为了安全，先关闭电机
                setMoterPower(MOTOR_POWER_OFF);
                break;
        }

        // knuPkt[7] 是尾灯
        setLight(knuPkt[7]);
    }

    private void FillPktHeadAndTail(byte[] knuPkt)
    {
        // 指定的包头
        knuPkt[0] = 0x01;
        knuPkt[1] = 0x55;

        // 指定的包尾 'ABCD'
        knuPkt[23] = 0x41;
        knuPkt[24] = 0x42;
        knuPkt[25] = 0x43;
        knuPkt[26] = 0x44;
    }

    private void FillChecksum(byte[] knuPkt)
    {
        // 偶数表示 KnuIo 板正常，奇数表示不正常
        knuPkt[22] = 0x02;

        // 校验 11 - 14 字节
        {
            byte tmpXorChecksum = 0x00;

            tmpXorChecksum ^= knuPkt[11];
            tmpXorChecksum ^= knuPkt[12];
            tmpXorChecksum ^= knuPkt[13];

            knuPkt[10] = tmpXorChecksum;
        }

        // 校验 15 - 17 字节
        {
            byte tmpXorChecksum = 0x00;

            tmpXorChecksum ^= knuPkt[15];
            tmpXorChecksum ^= knuPkt[16];
            tmpXorChecksum ^= knuPkt[17];

            knuPkt[14] = tmpXorChecksum;
        }

        // 总校验
        {
            byte tmpXorChecksum = 0x00;
            for (int idx = 2; idx < knuPkt.Length - 4; idx++) {
                if (idx == 8 || idx == 21) {
                    continue;
                }
                tmpXorChecksum ^= knuPkt[idx];
            }

            // 0x41 和 0x42 没什么特别，协议就是这么规定的
            tmpXorChecksum ^= 0x41; // EndRead_1;
            tmpXorChecksum ^= 0x42; // EndRead_2;

            knuPkt[21] = tmpXorChecksum;
        }
    }

    // 把读到的 GKIO 包 gkio[] 转换成摩托艇程序能识别的格式，放到 buf[] 中
    private void DecodeGkioToKnuIo(byte[] gkioPkt, byte[] knuPkt)
    {
        Array.Clear(knuPkt, 0, knuPkt.Length);

        // 填充包头包尾
        FillPktHeadAndTail(knuPkt);

        // GKIO 当前的 coin 数量，直接转换到摩托艇需要的格式
        {
            byte gkioCoin = gkioPkt[1];
            knuPkt[8] = gkioCoin;
        }

        // GKIO 的 4 - 7 字节都可以定义为按钮。我们选用 gkio[4] 和 gkio[5]，并转换到摩托艇需要的格式
        // 各 bit 为 1 表示按下，为 0 表示弹起
        {
            byte gkioButton = 0x00;

            // 注意 GKIO 是 1 表示未按，0 表示按下，所以要 ~ 取反
            // 第 4 个字节，玩家可用按钮，1 - 测试, 3 - 下移, 4 - 投币
            byte tmpButton = (byte)(~gkioPkt[4]);

            const byte MASK_TEST = 0x02;  // 0000,0010
            if ((byte)(tmpButton & MASK_TEST) == MASK_TEST) {
                // gkio 的 bit 1 对应 knuio 的 bit 4
                gkioButton |= 0x10; // 0001,0000
            }

            const byte MASK_MOVE = 0x08; // 0000,1000
            if ((byte)(tmpButton & MASK_MOVE) == MASK_MOVE) {
                // gkio 的 bit 3 对应 knuio 的 bit 5
                gkioButton |= 0x20; // 0010,0000
            }

            // 投币是由固件直接转换成币数，放在第 1 个字节

            tmpButton = (byte)(~gkioPkt[5]);
            const byte MASK_SPEAKER = 0x20;
            if ((byte)(tmpButton & MASK_SPEAKER) == MASK_SPEAKER) {
                // gkio 的 bit 1 对应 knuio 的 bit 5
                gkioButton |= 0x04;
            }

            tmpButton = (byte)(~gkioPkt[5]);
            const byte MASK_START = 0x40;
            if ((byte)(tmpButton & MASK_START) == MASK_START) {
                // gkio 的 bit 1 对应 knuio 的 bit 5
                gkioButton |= 0x01;
            }

            // gkio[7] 是板子上的拨码开关，支持使用拨码开关操作，便于调试和场地客人排错
            // 合成为 1 个 byte，放到摩托艇的 button 状态字节
            knuPkt[9] = (byte)((~gkioPkt[7]) | gkioButton);
        }

        // GKIO 读出的 3 个 8 bit 电位器数据 byte[8] byte[9] byte[10]，要转换成摩托艇的
        // 4 bit 高位 +8 bit 低位的 12 位格式。为了扩大精度到 12 bit，最后 4 bit 取 0
        {
            byte wheel = gkioPkt[8];
            knuPkt[6] = (byte)(wheel >> 4);   // 方向盘高位
            knuPkt[7] = (byte)(wheel << 4);   // 方向盘低位，最低 4-bit 补 0

            byte thrust = gkioPkt[9];
            knuPkt[2] = (byte)(thrust >> 4);  // 油门
            knuPkt[3] = (byte)(thrust << 4);

            // 摩托艇没有刹车，跳过了
        }

        FillChecksum(knuPkt);
    }

    private byte[] gkio_read_buf = new byte[16];
    public void Read(byte[] buf, int offset, int count)
    {
        if (buf.Length != 27) {
            //Debug.Log("buf.Length must be 27, but it is : " + buf.Length + ", count is : " + count);
            return;
        }

        // 最后读取一次 GKIO 状态
        {
            byte[] CMD_GET_STAT = {
                    0x61, 0x8A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };
            gkio_write_read(CMD_GET_STAT, gkio_read_buf);
        }

        // 80 38 38 38 FF FF FF FF 03 03 03 03 00 00 00 00
        //gkioutil.output_array("READ() gkio buf : ", gkio_read_buf);

        DecodeGkioToKnuIo(gkio_read_buf, buf);

        // 01 55, 包头
        // 00 30 00 30 00 30, 三个电位器, 03 变 30，8-bit 转 12-bit，正确
        // 38, 币数
        // 00, 按钮状态，正常
        // 00 00 00 00, byte[10] 校验和
        // 00 00 00 00, byte[14] 校验和
        // 00 00 00, 随机值
        // 32, byte[21] 校验和，应该为 31
        // 02, 必须为偶数
        // 41 42 43 44 包尾
        //gkioutil.output_array("READ() KnuIo buf : ", buf);
    }
}

public class MyCOMDevice : MonoBehaviour
{
    public class ComThreadClass
	{
		public string ThreadName;
        static SerialPort _SerialPort;
        //private static GkioPort _SerialPort;
        public static int BufLenRead = 27;
		public static int BufLenReadEnd = 4;
		public static  int BufLenWrite = 23;
		public static byte[] ReadByteMsg = new byte[BufLenRead];
		public static byte[] WriteByteMsg = new byte[BufLenWrite];
		static string RxStringData;
//		static string _NewLine = "ABCD"; //0x41 0x42 0x43 0x44
		public static int ReadTimeout = 0x0050; //单位为毫秒.
		public static int WriteTimeout = 0x07d0;
		public static bool IsStopComTX;
		public static bool IsReadMsgComTimeOut;
		public static string ComPortName = "COM1";
		public static bool IsReadComMsg;
		public static bool IsTestWRPer;
		public static int WriteCount;
		public static int ReadCount;
		public static int ReadTimeOutCount;

		public ComThreadClass(string name)
		{
			ThreadName = name;
			OpenComPort();
		}

        public static void OpenComPort()
		{
			if (!pcvr.bIsHardWare) {
				return;
			}

			if (_SerialPort != null) {
				return;
			}

            _SerialPort = new SerialPort(ComPortName, 38400, Parity.None, 8, StopBits.One);
            //_SerialPort = new GkioPort();
			if (_SerialPort != null)
			{
				try
				{
					if (_SerialPort.IsOpen)
					{
						_SerialPort.Close();
						Debug.Log("Closing port, because it was already open!");
					}
					else
					{
						_SerialPort.Open();
						if (_SerialPort.IsOpen) {
							IsFindDeviceDt = true;
							Debug.Log("COM open sucess");
                        }
					}
				}
				catch (Exception exception)
				{
					if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
						return;
					}
					Debug.Log("error:COM already opened by other PRG... " + exception);
				}
			}
			else
			{
				Debug.Log("Port == null");
			}
		}

		public void Run()
		{
			do
			{
                /*
                // 禁止这条，摩托艇可玩 1 局，然后 IO 板失灵
				if (XkGameCtrl.IsLoadingLevel) {
					Thread.Sleep(100);
					continue;
				}

                // 禁止后面这 2 条，摩托艇全程可玩，但不知道有没有副作用?
                IsTestWRPer = false;
				if (IsReadMsgComTimeOut) {
					CloseComPort();
					break;
				}

                if (IsStopComTX) {
					IsReadComMsg = false;
					Thread.Sleep(1000);
					continue;
				}
                */

                COMTxData();
				if (pcvr.IsJiaoYanHid || !pcvr.IsPlayerActivePcvr) {
					Thread.Sleep(100);
				}
				else {
					Thread.Sleep(25);
				}

                COMRxData();
				if (pcvr.IsJiaoYanHid || !pcvr.IsPlayerActivePcvr) {
					Thread.Sleep(100);
				}
				else {
					Thread.Sleep(25);
				}
				IsTestWRPer = true;
            }
			while (_SerialPort.IsOpen);
			CloseComPort();
			Debug.Log("Close run thead...");
		}

		void COMTxData()
		{
			if (XkGameCtrl.IsGameOnQuit) {
				return;
			}

			try
			{
				IsReadComMsg = false;
				_SerialPort.Write(WriteByteMsg, 0, WriteByteMsg.Length);
				WriteCount++;
			}
			catch (Exception exception)
			{
				if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
					return;
				}
				Debug.Log("Tx error:COM!!! " + exception);
			}
		}

		void COMRxData()
		{
			if (XkGameCtrl.IsGameOnQuit) {
				return;
			}

			try
			{
                _SerialPort.Read(ReadByteMsg, 0, ReadByteMsg.Length);
				ReadCount++;
				IsReadComMsg = true;
				ReadMsgTimeOutVal = 0f;
				CountOpenCom = 0;
			}
			catch (Exception exception)
			{
				if (XkGameCtrl.IsGameOnQuit || ComThread == null) {
					return;
				}

				Debug.Log("Rx error:COM..." + exception);
				IsReadMsgComTimeOut = true;
				IsReadComMsg = false;
				ReadTimeOutCount++;
			}
		}

		public static void CloseComPort()
		{
			IsReadComMsg = false;
			if (_SerialPort == null || !_SerialPort.IsOpen) {
				return;
			}
			_SerialPort.Close();
			_SerialPort = null;
		}
	}

	static ComThreadClass _ComThreadClass;
	static Thread ComThread;
	public static bool IsFindDeviceDt;
	public static float ReadMsgTimeOutVal;
	static float TimeLastVal;
	const float TimeUnitDelta = 0.1f; //单位为秒.
	public static uint CountRestartCom;
	public static uint CountOpenCom;
	static MyCOMDevice _Instance;

    public static MyCOMDevice GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_MyCOMDevice");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<MyCOMDevice>();
		}
		return _Instance;
	}
    
    // Use this for initialization
    void Start()
	{
		StartCoroutine(OpenComThread());
	}

	IEnumerator OpenComThread()
	{
		if (!pcvr.bIsHardWare) {
			yield break;
		}

		ReadMsgTimeOutVal = 0f;
		ComThreadClass.IsReadMsgComTimeOut = false;
		ComThreadClass.IsReadComMsg = false;
		ComThreadClass.IsStopComTX = false;
		if (_ComThreadClass == null) {
			_ComThreadClass = new ComThreadClass(ComThreadClass.ComPortName);
		}
		else {
			ComThreadClass.CloseComPort();
		}
		
		if (ComThread != null) {
			CloseComThread();
		}
		yield return new WaitForSeconds(2f);

		ComThreadClass.OpenComPort();
		if (ComThread == null) {
			ComThread = new Thread(new ThreadStart(_ComThreadClass.Run));
			ComThread.Start();
		}
	}

	//void RestartComPort()
	//{
	//	if (!ComThreadClass.IsReadMsgComTimeOut) {
	//		return;
	//	}
	//	CountRestartCom++;
	//	CountOpenCom++;
	//	ScreenLog.Log("Restart ComPort "+ComThreadClass.ComPortName+", time "+(int)Time.realtimeSinceStartup);
	//	ScreenLog.Log("CountRestartCom: "+CountRestartCom);
	//	StartCoroutine(OpenComThread());
	//}

	//void CheckTimeOutReadMsg()
	//{
	//	ReadMsgTimeOutVal += TimeUnitDelta;
		/*float timeMinVal = CountOpenCom < 6 ? 0.5f : 4f;
		if (CountOpenCom > 20) {
			timeMinVal = 10f;
		}*/

        /* czq
		if (ReadMsgTimeOutVal > timeMinVal) {
			ScreenLog.Log("CheckTimeOutReadMsg -> The app should restart to open the COM!");
			ComThreadClass.IsReadMsgComTimeOut = true;
			RestartComPort();
		}
        */
	//}

	// 强制重启串口通讯,目的是清理串口缓存信息.
	//public void ForceRestartComPort()
	//{
        /*
		if (!pcvr.bIsHardWare) {
			return;
		}
		ComThreadClass.IsReadMsgComTimeOut = true;
		RestartComPort();
        */
	//}

//	void Update()
//	{
		//test...
//		if (Input.GetKeyUp(KeyCode.T)) {
//			ForceRestartComPort();
//		}
//		if (Input.GetKeyUp(KeyCode.T)) {
//			XkGameCtrl.IsLoadingLevel = !XkGameCtrl.IsLoadingLevel;
//		}
		//test end...
		
//		if (!pcvr.bIsHardWare || XkGameCtrl.IsLoadingLevel || ComThreadClass.IsReadComMsg) {
//			return;
//		}
//		
//		if (Time.realtimeSinceStartup - TimeLastVal < TimeUnitDelta) {
//			return;
//		}
//		TimeLastVal = Time.realtimeSinceStartup;
//		CheckTimeOutReadMsg();
//	}

//	void OnGUI()
//	{
//		string strA = "IsReadComMsg "+ComThreadClass.IsReadComMsg
//			+", ReadMsgTimeOutVal "+ReadMsgTimeOutVal.ToString("f2");
//		GUI.Box(new Rect(0f, 0f, 400f, 25f), strA);
//	}

	void CloseComThread()
	{
		if (ComThread != null) {
			ComThread.Abort();
			ComThread = null;
		}
	}

	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit...Com");
		XkGameCtrl.IsGameOnQuit = true;
		ComThreadClass.CloseComPort();
		Invoke("CloseComThread", 2f);
	}
}