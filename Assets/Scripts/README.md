# 🎮 Unity Game System Overview

本專案使用 Unity 製作，以下為各主要腳本與資料夾結構的功能說明。

---

## 📁 Scripts

### 📷 CameraController.cs

> **狀態：** 已暫時停用  
> **備註：** 改用 Unity 的 **Cinemachine** 智慧相機進行鏡頭控制與跟隨。

---

### 🎮 GameManager.cs

負責管理整體遊戲流程與資料狀態。

- **啟動時行為：**
  - 自動載入遊戲資料並初始化。
  - 將資料儲存在 GameManager 的內部變數中。
- **存檔行為：**
  - 將目前的遊戲資料整理打包成 `GameData` 和 `PlayerData`。
  - 儲存至指定位置以供下次遊戲載入使用。

---

### 🧍 PlayerColliderController.cs

掛載於玩家子物件（如腳部或頭部感應器），負責偵測環境的物理碰撞。

- 判斷是否與「地板」、「牆壁」、「天花板」接觸。
- 控制 `PlayerController` 的碰撞狀態旗標，用於跳躍、爬牆等行為判斷。

---

### 🕹️ PlayerController.cs

掛載於玩家角色本體。

- 控制玩家的移動、跳躍、爬牆等操作。
- 管理玩家的生命值、狀態參數與動畫控制。

---

### 🟢 SafePoint.cs

掛載於「安全復活點」感應體上。

- 玩家觸碰後，記錄當前安全的站立位置。
- 傳回的位置資訊儲存在 `GameManager`，供復活時使用。

---

### 💾 SavePoint.cs

掛載於「存檔點」感應體上。

- 玩家靠近時觸發自動存檔邏輯。
- 將當前遊戲狀態儲存到本地存檔檔案。

---

### 🔁 ScenePortal.cs

掛載於切換場景的感應區域上。

- 玩家進入區域時觸發場景轉換邏輯。
- 支援轉場特效、音效與傳送點設定。

---

### ☠️ Spike.cs

掛載於會導致玩家死亡的陷阱物件（如尖刺）上。

- 玩家接觸時觸發死亡流程。
- 玩家會被傳送回最近的 `SafePoint`。

---

## 📂 Folder Structure 說明

### 📂 `dataControl` 資料夾

#### 📄 SaveManager.cs

- 提供玩家遊戲狀態的存檔與讀檔方法。
- 搭配自定義的 `GameData` 與 `PlayerData` 使用。

---

### 📂 `mainMenu` 資料夾

#### 📄 SaveSlotButton.cs

- 控制主選單中玩家點擊的「存檔選擇」按鈕。
- 玩家選擇後會載入對應的遊戲檔案。

---

### 📂 `SceneControl` 資料夾

#### 📄 AllScene.cs

- 每個場景共用的初始化邏輯。
- 包含生成玩家、設定初始位置、綁定 CineMachine 鏡頭等。

#### 📄 [場景名稱].cs

- 每個場景的專屬邏輯控制腳本（例如 `Level1.cs`, `CaveEntrance.cs`, `BossRoom.cs` 等）。
- 處理場景內的敵人生成、音樂、過場動畫與互動物件行為等。

---

如有更多模組與資料夾，可持續擴充本文件，並建議與版本控制系統同步更新。

