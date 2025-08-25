using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Normal,       // 可重複生成、可多次撿 (錢、素材)
    Unique,       // 遊戲中只有 1 個 (特殊能力)
    OneTime,  // 數量有限，撿了就消失 (心之容器)

}

