using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterGameStateController : MonoBehaviour {

    private SaveFileModel currentSaveFile;

    private SaveDataInfo infoBeforeBattle; 

    public static int MAX_SAVE_SLOTS = 6;

    #region Singleton
    public MasterGameStateController instance;

    public void Awake() {
       if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this.gameObject);
        }  
          
    }
    #endregion    

    public void SaveToFile(int slotId) {
        FileUtils.SaveToFile(slotId, currentSaveFile);
    }

    public void LoadFromFile(int slotId) {
        currentSaveFile = FileUtils.LoadFromFile(slotId);
    }

    public void CreateNewSaveInstance(PlayerInfo mainCharacter) {
        SaveDataInfo newSaveData = new SaveDataInfo(new QuestListData(), new PlayerInfoListData(), new Inventory(), 100);
        currentSaveFile = new SaveFileModel(newSaveData);
        AddNewPlayer(mainCharacter);
    }

    public void UpdateQuestListData(List<Quest> availableQuests, List<int> completedQuestIds) {
        currentSaveFile.saveData.playerQuestData.availableQuests = availableQuests;
        currentSaveFile.saveData.playerQuestData.completedQuestIds = completedQuestIds;
    }

    public void UpdatePlayerInfoListData(List<PlayerInfo> playerInfoList) {
        currentSaveFile.saveData.playerInfoData.playerInfoDataList = playerInfoList;
    }

    public void CompleteQuest(Quest completedQuest) {
        currentSaveFile.saveData.playerQuestData.completedQuestIds.Add(completedQuest.questId);
    }

    public void AddNewPlayer(PlayerInfo newPlayer) {
        currentSaveFile.saveData.playerInfoData.playerInfoDataList.Add(newPlayer);
    }

    public void UpdatePlayerInfo(PlayerInfo playerInfo) {
        List<PlayerInfo> playerInfoToUpdateList = new List<PlayerInfo>(){playerInfo};
        UpdatePlayerInfos(playerInfoToUpdateList);
    }

    public void UpdatePlayerInfos(List<PlayerInfo> playerInfosToUpdate) {
        List<PlayerInfo> data = currentSaveFile.saveData.playerInfoData.playerInfoDataList;
        foreach (PlayerInfo newPInfo in playerInfosToUpdate) {
            for (int idx = 0; idx < data.Count; idx++) {
                if (newPInfo.id.Equals(data[idx].id)) {
                    data[idx] = newPInfo;
                }    
            }            
        }
        UpdatePlayerInfoListData(data);
    }

    public List<PlayerInfo> GetAllPlayerInfos() {
        return currentSaveFile.saveData.playerInfoData.playerInfoDataList;
    }

    public PlayerInfo GetPlayerInfoById(string id) {
        foreach (PlayerInfo player in GetAllPlayerInfos()) {
            if (player.id.Equals(id)) {
                return player;
            }
        }
        return null;
    }

    public List<int> GetPlayerCompletedQuestIds() {
        return currentSaveFile.saveData.playerQuestData.completedQuestIds;
    }

    public List<ConsumableItem> GetAllConsumableItems() {
        return currentSaveFile.saveData.playerInventory.GetAllConsumableItems();
    }

    public List<EquipmentItem> GetAllEquipmentItems() {
        return currentSaveFile.saveData.playerInventory.GetAllEquipmentItems();
    }

    public void AddItemToInventory(ConsumableItem item) {
        currentSaveFile.saveData.playerInventory.AddItem(item);
    }

    public void RemoveItemFromInventory(ConsumableItem item) {
        currentSaveFile.saveData.playerInventory.RemoveItem(item);
    }

    public void AddItemToInventory(EquipmentItem item) {
        currentSaveFile.saveData.playerInventory.AddItem(item);
    }

    public void RemoveItemFromInventory(EquipmentItem item) {
        currentSaveFile.saveData.playerInventory.RemoveItem(item);
    }

    public int GetPlayerGold() {
        return currentSaveFile.saveData.playerGold;
    }

    public void SetPlayerGold(int gold) {
        currentSaveFile.saveData.playerGold = gold;
    }

    public void AddPlayerGold(int gold) {
        currentSaveFile.saveData.playerGold += gold;
    }

    public void SubtractPlayerGold(int gold) {
        currentSaveFile.saveData.playerGold -= gold;
        currentSaveFile.saveData.playerGold = Mathf.Max(0, currentSaveFile.saveData.playerGold);
    }

    public void SaveInfoBeforeBattle() {
        SaveDataInfo currentSaveData = currentSaveFile.saveData;
        SaveDataInfo storeSaveData = new SaveDataInfo(currentSaveData.playerQuestData, currentSaveData.playerInfoData, currentSaveData.playerInventory, currentSaveData.playerGold);
        infoBeforeBattle = storeSaveData;
    }

    public void RevertDataToBeforeBattle() {
        currentSaveFile.saveData = infoBeforeBattle;
        ClearInfoBeforeBattleData();
    }

    public void ClearInfoBeforeBattleData() {
        infoBeforeBattle = null;
    }
    
}
