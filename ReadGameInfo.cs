#define USE_HANDLE_JSON

using UnityEngine;
using System;
using System.Collections.Generic;

using Knu;

public class ReadGameInfo : MonoBehaviour
{
    const int STM32_ADC_MAX = 4096;

    private knuconfig kconf_;

    private void Awake()
    {
        kconf_ = new knuconfig(@"mtt.conf", ItemToProperty, PropertyToItem);
        kconf_.loadFromFile();
    }

    // knuconfig 会遍历配置文件的每一行，依次解析出设定项目和值，并传递给此函数
    // 此函数根据情况，将字符串解析成属性的真实类型
    private void ItemToProperty(string name, string value)
    {
        try {
            /*if (String.Equals(name, "coin_to_start")) {
                coin_to_start = int.Parse(value);
            }
            */
        }
        catch {
            // 解码配置文件时出错，下一行继续解码。场地可使用 "恢复工厂默认" 功能修正此问题
        }
    }

    // 把属性的值，保存到字符串数组，准备写入配置文件
    private string[] PropertyToItem()
    {
        List<string> item = new List<string>();

        // 高版本 C# 可以用防拼写错误的类型安全写法 nameof(volumn)，unity3d 的 mono 不支持这个语法
        //item.Add(String.Format("{0} = {1}", "coin_to_start", coin_to_start));

        return item.ToArray();
    }

    private static ReadGameInfo Instance = null;
    static public ReadGameInfo GetInstance()
    {
        if (Instance == null) {
            GameObject obj = new GameObject("_ReadGameInfo");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<ReadGameInfo>();
        }

        return Instance;
    }

    private int START_COIN;

    const int MODE_OPERATOR = 0;
    const int MODE_FREEPLAY = 1;
    private int GAME_MODE = MODE_OPERATOR;
    public int GameMode {
        get
        {
            return GAME_MODE;
        }
        set
        {
            if (value == MODE_FREEPLAY) {
                GAME_MODE = MODE_FREEPLAY;
            }
            else {
                GAME_MODE = MODE_OPERATOR;
            }
        }
    }

    private int INSERT_COIN;
    public int InserCoin {
        get
        {
            return INSERT_COIN;
        }
        set
        {
            INSERT_COIN = Math.Max(value, 0);
        }
    }

    private int GAME_RECORD;
    public int GameRecord
    {
        get
        {
            return GAME_RECORD;
        }
        set
        {
            GAME_RECORD = Math.Max(value, 0);
        }
    }

    private int PLAYER_SPEED_MIN;
    public int PlayerSpeedMin
    {
        get
        {
            return PLAYER_SPEED_MIN;
        }
        set
        {
            PLAYER_SPEED_MIN = Mathf.Clamp(value, 0, 80);
        }
    }

    [Obsolete("ReadPlayerMinSpeedVal() has been deprecated. Use property PlayerSpeedMin instead.")]
    public int ReadPlayerMinSpeedVal()
    {
        return 0;
    }

    public void WritePlayerMinSpeedVal(int value)
    {

    }

    private int AUDIO_VOLUME;
    public int AudioVolume
    {
        get
        {
            return AUDIO_VOLUME;
        }
        set
        {
            AUDIO_VOLUME = Mathf.Clamp(value, 0, 10);
        }
    }

    private int BIKE_STEER_MIN;     // BikeDirMin;
    public int BikeSteerMin
    {
        get
        {
            return BIKE_STEER_MIN;
        }
        set
        {
            BIKE_STEER_MIN = Mathf.Clamp(value, 0, BIKE_STEER_MIDDLE);
        }
    }

    private int BIKE_STEER_MIDDLE;  // BikeDirCen;
    public int BikeSteerMiddle
    {
        get
        {
            return BIKE_STEER_MIDDLE;
        }
        set
        {
            BIKE_STEER_MIDDLE = Mathf.Clamp(value, BIKE_STEER_MIN, BIKE_STEER_MAX);
        }
    }

    private int BIKE_STEER_MAX;
    public int BikeSteerMax
    {
        get
        {
            return BIKE_STEER_MAX;
        }
        set
        {
            BIKE_STEER_MAX = Mathf.Clamp(value, BIKE_STEER_MIDDLE, STM32_ADC_MAX);
        }
    }

    private int BIKE_THRUST_MIN;    // BikePowerMin
    public int BikeThrustMin
    {
        get
        {
            return BIKE_THRUST_MIN;
        }
        set
        {
            BIKE_THRUST_MIN = Mathf.Clamp(value, 0, BIKE_THRUST_MAX);
        }
    }

    private int BIKE_THRUST_MAX;
    public int BikeThrustMax
    {
        get
        {
            return BIKE_THRUST_MAX;
        }
        set
        {
            BIKE_THRUST_MAX = Mathf.Clamp(value, BIKE_THRUST_MIN, STM32_ADC_MAX);
        }
    }

    private int BIKE_BRAKE_MIN;     // BikeShaCheMin
    public int BikeBrakeMin
    {
        get
        {
            return BIKE_BRAKE_MIN;
        }
        set
        {
            BIKE_BRAKE_MIN = Mathf.Clamp(value, 0, BIKE_THRUST_MAX);
        }
    }

    private int BIKE_BRAKE_MAX;
    public int BikeBrakeMax
    {
        get
        {
            return BIKE_BRAKE_MAX;
        }
        set
        {
            BIKE_BRAKE_MAX = Mathf.Clamp(value, BIKE_BRAKE_MIN, STM32_ADC_MAX);
        }
    }

    void InitGameInfo()
	{
		
	}

	public void FactoryReset()
	{

	}

	public string ReadStarCoinNumSet()
	{
        return "1";
	}

    public string ReadGameStarMode()
	{
        return "expr";
	}

    public string ReadInsertCoinNum()
	{
        return "0";
	}

    public int ReadGameRecord()
	{
        return 1;
	}

    public void WriteStarCoinNumSet(string value)
	{

    }

    public void WriteGameStarMode(string value)
	{

	}

    public void WriteInsertCoinNum(string value)
	{

	}

    public void WriteGameRecord(int value)
	{

	}

    public int ReadPlayerMinSpeedVal()
	{
        return 0;
	}

    public void WritePlayerMinSpeedVal(int value)
	{

	}
}