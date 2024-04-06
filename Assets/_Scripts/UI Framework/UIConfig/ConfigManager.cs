using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIFramework
{
    public interface IConfig
    {
        Dictionary<string, string> ConfigSettingDic { get; }
        int GetConfigSettingMaxNumber();
    }

    [System.Serializable]
    internal class KeyValueInfo
    {
        public List<KeyValueNode> configInfoList = null;
    }

    [System.Serializable]
    internal class KeyValueNode
    {
        //鍵
        public string key = null;
        //值
        public string value = null;
    }

    /// <summary>
    /// 為ConfigManager自定義異常
    /// 注意：Json文件中的開頭名稱(例如：configInfoList)必須與KeyValueInfo中的List變量名稱(configInfoList)相同，否則會出鏪
    /// </summary>
    public class JsonAnlysisException : Exception
    {
        public JsonAnlysisException() : base() { }

        public JsonAnlysisException(string exceptionMsg) : base(exceptionMsg) { }
    }

    public class ConfigManager : IConfig
    {
        private Dictionary<string, string> configSettingDic = null;
        public Dictionary<string, string> ConfigSettingDic { get => configSettingDic; }

        public ConfigManager(string jsonPath)
        {
            configSettingDic = new Dictionary<string, string>();
            InitAndAnalysisJson(jsonPath);
        }

        private void InitAndAnalysisJson(string jsonPath)
        {
            TextAsset config = null;
            KeyValueInfo keyValueInfo = null;

            if (string.IsNullOrEmpty(jsonPath))
                return;

            try
            {
                config = Resources.Load<TextAsset>(jsonPath);
                keyValueInfo = JsonUtility.FromJson<KeyValueInfo>(config.text);
            }
            catch
            {
                //拋自定義異常
                throw new JsonAnlysisException($"{GetType()}/InitAndAnalysisJson()/Json Analysis Exception !!! Parameter jsonPath = {jsonPath}");
            }

            foreach (KeyValueNode node in keyValueInfo.configInfoList)
                configSettingDic.Add(node.key, node.value);
        }

        public int GetConfigSettingMaxNumber()
        {
            if (configSettingDic != null && configSettingDic.Count > 0)
                return configSettingDic.Count;

            return 0;
        }
    }
}