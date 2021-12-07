using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class WordManager : MonoBehaviour
{
    private static WordManager instance;
    public static WordManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    public string filePath = "Assets/Resources/Data.txt";

    private Dictionary<string, List<string>> activeWordDict = new Dictionary<string, List<string>>();

    private Dictionary<string, List<string>> deActiveWordDict = new Dictionary<string, List<string>>();


    public void LoadData()
    {
        FileInfo fileInfo = new FileInfo(filePath);

        if (fileInfo.Exists)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                //Debug.Log(reader.ReadToEnd());
                bool isDeactiveWord = false;
                while (true)
                {
                    string lineValue = reader.ReadLine();
                    Debug.LogWarning("lineValue : " + lineValue);
                    if (string.IsNullOrEmpty(lineValue))
                        break;

                    if (!isDeactiveWord && string.Equals(lineValue, DEACTIVE_KEY))
                    {
                        isDeactiveWord = true;
                        continue;
                    }

                    string key = lineValue.Substring(0, 1);
                    string ckey = lineValue[0].ToString();
                    Debug.Log("key : " + key + " ckey : " + ckey);
                    if (isDeactiveWord)
                    {
                        if (!deActiveWordDict.TryGetValue(key, out var delist))
                        {
                            if (delist == null)
                            {
                                delist = new List<string>();
                                Debug.LogError("delistdelistdelist");
                            }
                            deActiveWordDict.Add(key, delist);
                        }

                        delist.Add(lineValue);
                    }
                    else
                    {
                        if (!activeWordDict.TryGetValue(key, out var list))
                        {
                            if (list == null)
                            {
                                list = new List<string>();
                                Debug.LogError("lllllllllll");
                            }
                            activeWordDict.Add(key, list);
                        }

                        list.Add(lineValue);
                    }
                }

                reader.Close();
            }

            
        }
    }

    private const string DEACTIVE_KEY = "DEACTIVE WORD";

    private const string ACTIVE_KEY = "ACTIVE WORD";

    public void SaveData()
    {
        
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.Unicode);

        //writer.WriteLine(ACTIVE_KEY);
        var keys = activeWordDict.Keys.GetEnumerator();
        while (keys.MoveNext())
        {
            for (int i = 0; i < activeWordDict[keys.Current].Count; i++)
            {
                writer.WriteLine(activeWordDict[keys.Current][i]);
            }
        }

        writer.WriteLine(DEACTIVE_KEY);
        var dekeys = deActiveWordDict.Keys.GetEnumerator();
        while (dekeys.MoveNext())
        {
            for (int i = 0; i < deActiveWordDict[dekeys.Current].Count; i++)
            {
                writer.WriteLine(deActiveWordDict[dekeys.Current][i]);
            }
        }

        writer.Close();
    }



    public bool IsRegistedWord(string word)
    {
        if (activeWordDict.TryGetValue(word.Substring(0, 1), out var list))
        {
            if (list.Contains(word))
                return true;
        }

        if (deActiveWordDict.TryGetValue(word.Substring(0, 1), out var delist))
        {
            if (delist.Contains(word))
                return true;
        }

        return false;
    }

    public void AddActiveWord(string word)
    {
        string key = word.Substring(0);
        if (!activeWordDict.TryGetValue(key, out _))
        {
            activeWordDict.Add(key, new List<string>());
            activeWordDict[key].Add(word);
        }

        if (!activeWordDict[key].Contains(word))
            activeWordDict[key].Add(word);
    }

    public void AddDeActiveWord(string word)
    {
        string key = word.Substring(0);
        if (!deActiveWordDict.TryGetValue(key, out _))
        {
            deActiveWordDict.Add(key, new List<string>());
            deActiveWordDict[key].Add(word);
        }

        if (!deActiveWordDict[key].Contains(word))
            deActiveWordDict[key].Add(word);
    }



}
