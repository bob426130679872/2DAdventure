using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Item/Diary")]
public class DiaryTemplate : ItemTemplate {
    [TextArea(5,10)] public string diaryText;   // 日記內容 (中文)
    [TextArea(5,10)] public string diaryTextEN; // 日記內容 (English)
    public string diaryDate;
}
