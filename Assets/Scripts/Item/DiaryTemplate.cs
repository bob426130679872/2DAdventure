using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Item/Diary")]
public class DiaryTemplate : ItemTemplate {
    [TextArea(5,10)] public string diaryText;
    public string diaryDate;
}
