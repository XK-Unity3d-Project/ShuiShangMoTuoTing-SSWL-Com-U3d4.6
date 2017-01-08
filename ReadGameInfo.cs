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
    private int CoinToStart
    {
        get { return Math.Max(1, START_COIN); }
        set { START_COIN = Math.Max(value, 1); }
    }

    const int MODE_OPERATOR = 0;
    const int MODE_FREEPLAY = 1;
    private int GAME_MODE = MODE_OPERATOR;
    private int GameMode
    {
        get { return GAME_MODE; }
        set { GAME_MODE = (value == MODE_FREEPLAY) ? MODE_FREEPLAY : MODE_OPERATOR; }
    }

    private int INSERT_COIN;
    private int InsertCoin
    {
        get { return Math.Max(INSERT_COIN, 0); }
        set { INSERT_COIN = Math.Max(value, 0); }
    }

    private int GAME_RECORD;
    private int GameRecord
    {
        get { return Math.Max(GAME_RECORD, 0); }
        set { GAME_RECORD = Math.Max(value, 0); }
    }    

    private int PLAYER_SPEED_MIN;
    private int PlayerSpeedMin
    {
        get { return PLAYER_SPEED_MIN; }
        set { PLAYER_SPEED_MIN = Mathf.Clamp(value, 0, 80); }
    }

    private int AUDIO_VOLUME;
    private int AudioVolume
    {
        get { return Mathf.Clamp(AUDIO_VOLUME, 0, 10); }
        set { AUDIO_VOLUME = Mathf.Clamp(value, 0, 10); }
    }    

    private int STEER_MIN;     // BikeDirMin;
    public int SteerMin
    {
        get { return Mathf.Clamp(STEER_MIN, 0, STEER_CENTER); }
        set { STEER_MIN = Mathf.Clamp(value, 0, STEER_CENTER); }
    }

    private int STEER_CENTER;  // BikeDirCen;
    public int SteerCenter
    {
        get { return STEER_CENTER; }
        set { STEER_CENTER = Mathf.Clamp(value, STEER_MIN, STEER_MAX); }
    }

    private int STEER_MAX;
    public int SteerMax
    {
        get { return STEER_MAX; }
        set { STEER_MAX = Mathf.Clamp(value, STEER_CENTER, STM32_ADC_MAX); }
    }

    private int THRUST_MIN;    // BikePowerMin
    public int ThrustMin
    {
        get { return THRUST_MIN; }
        set { THRUST_MIN = Mathf.Clamp(value, 0, THRUST_MAX); }
    }

    private int THRUST_MAX;
    public int ThrustMax
    {
        get { return THRUST_MAX; }
        set { THRUST_MAX = Mathf.Clamp(value, THRUST_MIN, STM32_ADC_MAX); }
    }

    private int BRAKE_MIN;     // BikeShaCheMin
    public int BrakeMin
    {
        get { return BRAKE_MIN; }
        set { BRAKE_MIN = Mathf.Clamp(value, 0, THRUST_MAX); }
    }

    private int BRAKE_MAX;
    public int BrakeMax
    {
        get { return BRAKE_MAX; }
        set { BRAKE_MAX = Mathf.Clamp(value, BRAKE_MIN, STM32_ADC_MAX); }
    }

    void InitGameInfo()
	{
		
	}

    public void FactoryReset()
    {

    }

    /* 为了兼容性保留的废弃旧函数 */

    [Obsolete("ReadStarCoinNumSet has been deprecated. Use property CoinToStart instead.")]
    public string ReadStarCoinNumSet()
    {
        return Convert.ToString(CoinToStart);
    }
    [Obsolete("WriteStarCoinNumSet has been deprecated. Use property CoinToStart instead.")]
    public void WriteStarCoinNumSet(string value)
    {
        int tmp_coin_to_start;

        bool ParseSuccess = int.TryParse(value, out tmp_coin_to_start);

        if (ParseSuccess) {
            CoinToStart = tmp_coin_to_start;
        }
        else {
            tmp_coin_to_start = 0;
        }
    }

    [Obsolete("ReadGameStarMode has been deprecated. Use property GameMode instead.")]
    public string ReadGameStarMode()
    {
        return (GameMode == MODE_FREEPLAY) ? "FREE" : "oper";
    }
    [Obsolete("WriteGameStarMode has been deprecated. Use property GameMode instead.")]
    public void WriteGameStarMode(string mode_str)
    {
        GameMode = mode_str.Equals("FREE") ? MODE_FREEPLAY : MODE_OPERATOR;
    }

    [Obsolete("ReadInsertCoinNum has been deprecated. Use property InsertCoin instead.")]
    public string ReadInsertCoinNum()
    {
        return Convert.ToString(InsertCoin);
    }
    [Obsolete("WriteInsertCoinNum has been deprecated. Use property InsertCoin instead.")]
    public void WriteInsertCoinNum(string value)
    {
        int tmp_InsertCoin;

        bool ParseSuccess = int.TryParse(value, out tmp_InsertCoin);

        if (!ParseSuccess) {
            InsertCoin = tmp_InsertCoin;
        }
        else {
            InsertCoin = 0;
        }
    }

    [Obsolete("ReadPlayerMinSpeedVal has been deprecated. Use property PlayerSpeedMin instead.")]
    public int ReadPlayerMinSpeedVal()
    {
        return PlayerSpeedMin;
    }
    [Obsolete("WritePlayerMinSpeedVal has been deprecated. Use property PlayerSpeedMin instead.")]
    public void WritePlayerMinSpeedVal(int value)
    {
        PlayerSpeedMin = value;
    }

    [Obsolete("ReadGameRecord has been deprecated. Use property GameRecord instead.")]
    public int ReadGameRecord()
    {
        return GameRecord;
    }
    [Obsolete("WriteGameRecord has been deprecated. Use property GameRecord instead.")]
    public void WriteGameRecord(int value)
    {
        GameRecord = value;
    }

    [Obsolete("WriteGameAudioVolume has been deprecated. Use property AudioVolume instead.")]
    public void WriteGameAudioVolume(int value)
    {
        AudioVolume = value;
    }
    [Obsolete("ReadGameAudioVolume has been deprecated. Use property AudioVolume instead.")]
    public int ReadGameAudioVolume()
    {
        return AudioVolume;
    }
}