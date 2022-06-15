using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class FileUtils {

    private static string SAVE_FILE_PATH = "Assets/Resources/Saves/";
    private static string SAVE_FILE_NAME = "Slot_{0}_SaveFile";
    private static string LEVEL_FILE_EXT_JSON = ".json";
    private static ILogger logger = Debug.unityLogger;

    public static List<T> GetAssetsAtPath<T>(string path) {
        
        List<T> contentsList = new List<T>();
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/Resources/" + path);
 
        foreach (string fileName in fileEntries) {
            string sanitizedFileName = fileName.Replace("\\", "/");
            int assetPathIndex = sanitizedFileName.IndexOf("Resources/") + 10;
            string localPath = sanitizedFileName.Substring(assetPathIndex);
            string fileNoExt = Path.GetFileNameWithoutExtension(localPath);
            string[] splitFilePath = localPath.Split("/");
            string finalPath = "";
            for (int idx = 0; idx < splitFilePath.Length - 1; idx++) {
                finalPath += splitFilePath[idx] + "/";
            }
            finalPath += fileNoExt;
            //Debug.Log(finalPath);
            Object t = Resources.Load(finalPath);
            //Debug.Log(t);
 
            if (t != null) {
                al.Add(t); 
            }
                
        }
        
        for (int i = 0; i < al.Count; i++) {
            contentsList.Add((T)al[i]);
        }
            
        return contentsList;
    }

    public static string GenerateFileNameForSlotId(int slotId) {
        return SAVE_FILE_PATH + System.String.Format(SAVE_FILE_NAME,slotId) + ".json";
    }

    public static bool FileSlotExists(int slotId) {
        string filePath = GenerateFileNameForSlotId(slotId);
        return File.Exists(filePath);
    }

    public static string LoadTimeStampFromSlot(int slotId) {
        string filePath = GenerateFileNameForSlotId(slotId);
        StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
        string jsonText = streamReader.ReadToEnd();
        streamReader.Close();
        SaveFileModel saveModel = JsonConvert.DeserializeObject<SaveFileModel>(jsonText);  
        return saveModel.saveTimestamp;      
    }

    public static SaveFileModel LoadFromFile(int slotId) {
        string filePath = GenerateFileNameForSlotId(slotId);
        try {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            string jsonText = streamReader.ReadToEnd();
            streamReader.Close();
            SaveFileModel saveModel = JsonConvert.DeserializeObject<SaveFileModel>(jsonText);
            saveModel.saveData.Deserialize();
            return saveModel;           
        } catch (FileNotFoundException fileNotFoundException) {
            logger.LogError("Game", "File not found: " + filePath + " " + fileNotFoundException.Message.ToString());
            throw;
        } catch (System.Exception unexpectedException) {
            logger.LogError("Game", "Unexpected Exception while deserializing json: " + unexpectedException.Message.ToString());
            throw;
        }
    }

    public static void SaveToFile(int slotId, SaveFileModel updatedSaveModel) {
        SaveDataInfo oldSaveInfo = updatedSaveModel.saveData;
        SaveDataInfo newSaveInfo = new SaveDataInfo(oldSaveInfo.playerQuestData, oldSaveInfo.playerInfoData, oldSaveInfo.playerInventory, oldSaveInfo.playerGold);
        SaveFileModel newSaveFile = new SaveFileModel(newSaveInfo);
        string filePath = GenerateFileNameForSlotId(slotId);
        newSaveFile.saveFileName = filePath;
        newSaveFile.saveTimestamp = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        newSaveFile.saveData.Serialize();
        string json = JsonConvert.SerializeObject(newSaveFile);
        System.IO.File.WriteAllText(filePath, json);
    }

    public static List<Vector2> GetValidPlayerPlacements(string fileName) {
        if (!fileName.Contains(LEVEL_FILE_EXT_JSON)) {
            fileName += LEVEL_FILE_EXT_JSON;
        }
        try {
            StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
            string jsonText = streamReader.ReadToEnd();
            streamReader.Close();
            LevelFileModel levelFileModel = JsonConvert.DeserializeObject<LevelFileModel>(jsonText);
            int yStart = levelFileModel.playerPlacementRangeY[0];
            int yEnd = levelFileModel.playerPlacementRangeY[1];
            int xStart = levelFileModel.playerPlacementRangeX[0];
            int xEnd = levelFileModel.playerPlacementRangeX[1];

            List<Vector2> validPositions = new List<Vector2>();

            for (int x = xStart; x <= xEnd; x++) {
                for (int y = yStart; y <= yEnd; y++) {
                    validPositions.Add(new Vector2(x, y));
                }
            }
            return validPositions;
        } catch (FileNotFoundException fileNotFoundException) {
            logger.LogError("Game", "File not found: " + fileName + " " + fileNotFoundException.Message.ToString());
            throw;
        } catch (System.Exception unexpectedException) {
            logger.LogError("Game", "Unexpected Exception while deserializing json: " + unexpectedException.Message.ToString());
            throw;
        } 
    }

    public static int GetMaxNumberActivePlayers(string fileName) {
        if (!fileName.Contains(LEVEL_FILE_EXT_JSON)) {
            fileName += LEVEL_FILE_EXT_JSON;
        }
        try {
            StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
            string jsonText = streamReader.ReadToEnd();
            streamReader.Close();
            LevelFileModel levelFileModel = JsonConvert.DeserializeObject<LevelFileModel>(jsonText);
            return levelFileModel.maxActivePlayers;
        } catch (FileNotFoundException fileNotFoundException) {
            logger.LogError("Game", "File not found: " + fileName + " " + fileNotFoundException.Message.ToString());
            throw;
        } catch (System.Exception unexpectedException) {
            logger.LogError("Game", "Unexpected Exception while deserializing json: " + unexpectedException.Message.ToString());
            throw;
        }        
    }
    
}