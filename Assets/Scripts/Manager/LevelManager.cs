using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]


public class UnitPosition//鍏朵粬鍗曚綅浣嶇疆
{
    [Header("鍗曚綅绫诲瀷")]
    [SerializeField] private MapCellContent mapCellContent;//鍗曚綅绫诲瀷
    [Header("鍗曚綅鍒濆浣嶇疆")]
    [SerializeField] private Vector2Int position;//鍗曚綅鍒濆浣嶇疆
    [Header("鍗曚綅ID(鐪嬫敞閲?")]
    [SerializeField] private string id = "0";//鍗曚綅ID
    /*闂ㄥ拰閽ュ寵锛堣笍鏉匡級鍗曚綅瑕佺敤鍚屼竴涓猧d锛屽惁鍒欎細瀵艰嚧闂ㄥ拰閽ュ寵鏃犳硶鍖归厤銆俰d鐨勬牸寮忎竴鍏?浣嶏紝3浣嶅垎鍒搴?
    闂ㄥ拰閽ュ寵鐨剅gb鍊硷紝浣垮緱绯荤粺鑳藉鐢熸垚绗﹀悎瑙嗚琛ㄧ幇鐨勯棬鍜岄挜鍖欍€傞櫎寮€闂ㄥ拰閽ュ寵鐨刬d闇€瑕佹墜鍔ㄨ缃负0*/
    public MapCellContent getMapCellContent => mapCellContent;//鑾峰彇鍗曚綅绫诲瀷
    public Vector2Int getPosition => new Vector2Int(position.y, position.x);//鑾峰彇鍗曚綅鍒濆浣嶇疆
    public string getId => id;//鑾峰彇鍗曚綅ID
}
[System.Serializable]
public class CardDataManagement//鍏冲崱鐨勫崱鐗屽唴瀹?
{
    [Header("鍗＄墝鍐呭")]
    [SerializeField] private PlayerState cardType;//鍗＄墝鍐呭
    public PlayerState getCardType => cardType;//鑾峰彇鍗＄墝绫诲瀷
    [Header("绗嚑鍛ㄧ洰鑾峰緱")]
    [SerializeField] private int round = 1;//绗嚑鍥炲悎鑾峰緱
    [Header("浣跨敤鍑犳")]
    [SerializeField] private int useCount = 1;//浣跨敤鍑犳
    public int getUseCount => useCount;//鑾峰彇浣跨敤鍑犳
    public int getRound => round;//鑾峰彇绗嚑鍥炲悎鑾峰緱
}

[System.Serializable]
public class LevelManagement//鍌ㄥ瓨鍏冲崱淇℃伅鐨勬暟鎹粨鏋?
{
    [Header("鐜╁鍒濆浣嶇疆")]
    [SerializeField] private Vector2Int playerStartPosition;//鐜╁鍒濆浣嶇疆
    [Header("鍏朵粬鍗曚綅浣嶇疆")]
    [SerializeField] private List<UnitPosition> unitPositions = new List<UnitPosition>();//鍏朵粬鍗曚綅浣嶇疆
    [Header("鍦板浘澶у皬")]
    [SerializeField] private Vector2Int mapSize;//鍦板浘澶у皬
    [Header("End Step Count")]
    [SerializeField] private int endStepCount = 1;//闇€瑕佽蛋杩囧嚑娆＄粓鐐?
    [Header("Level Card Data")]
    [SerializeField] private List<CardDataManagement> cardData = new List<CardDataManagement>();//鍏冲崱鐨勫崱鐗屽唴瀹?
    [Header("Is Collapse Level")]
    [SerializeField] private bool isCollapse = false;//鏄惁涓哄闄峰叧鍗?
    public bool getIsCollapse => isCollapse;//鑾峰彇鏄惁涓哄闄峰叧鍗?
    public int getEndStepCount => endStepCount;//鑾峰彇闇€瑕佽蛋杩囧嚑娆＄粓鐐?
    public Vector2Int getMapSize => mapSize;//鑾峰彇鍦板浘澶у皬
    public List<UnitPosition> getUnitPositions => unitPositions;//鑾峰彇鍏朵粬鍗曚綅浣嶇疆
    public Vector2Int getPlayerStartPosition => new Vector2Int(playerStartPosition.y, playerStartPosition.x);//鑾峰彇鐜╁鍒濆浣嶇疆
    public List<CardDataManagement> getCardData => cardData;//鑾峰彇鍏冲崱鐨勫崱鐗屽唴瀹?
}
[System.Serializable]
public class ChapterManagement//鍌ㄥ瓨绔犺妭淇℃伅鐨勬暟鎹粨鏋?
{
    [Header("鍏冲崱淇℃伅")]
    [SerializeField] private List<LevelManagement> levelManagements = new List<LevelManagement>();//鍏冲崱淇℃伅
    public List<LevelManagement> getLevelManagement => levelManagements;//鑾峰彇鍏冲崱淇℃伅
}





[System.Serializable]
public class LevelManager : SingletonMono<LevelManager>
{
    [Header("绔犺妭淇℃伅")]
    [SerializeField] private List<ChapterManagement> Chapter = new List<ChapterManagement>();//绔犺妭缂栧彿
    [Header("浼犲叆鍗＄墝鍐呭")]
    [SerializeField] private CardDataSO cardData;//浼犲叆鍗＄墝鍐呭
    public List<CardData> getCardData => cardData.getCardList;//鑾峰彇浼犲叆鍗＄墝鍐呭
    public int getChapterNumber => Chapter.Count;//鑾峰彇绔犺妭鏁伴噺
    public ChapterManagement getChapter => Chapter[GameManager.Instance.getChapterNumber - 1];//鑾峰彇绔犺妭缂栧彿

    public LevelManagement GetLevelManagement(int chapterNumber, int levelNumber)//鑾峰彇鍏冲崱淇℃伅
    {
        return Chapter[chapterNumber - 1].getLevelManagement[levelNumber - 1];
    }

    public bool HasLevel(int chapterNumber, int levelNumber)
    {
        return chapterNumber >= 1 &&
               chapterNumber <= Chapter.Count &&
               levelNumber >= 1 &&
               levelNumber <= Chapter[chapterNumber - 1].getLevelManagement.Count;
    }

    public int GetLevelCount(int chapterNumber)
    {
        if (chapterNumber < 1 || chapterNumber > Chapter.Count)
        {
            return 0;
        }

        return Chapter[chapterNumber - 1].getLevelManagement.Count;
    }

    public List<CardData> GetCardData(int chapterNumber, int levelNumber, int endStepCount)//鑾峰彇鍏冲崱鐨勫崱鐗屽唴瀹?
    {
        List<CardData> cardDataList = new List<CardData>();//鍗＄墝鏁版嵁
        List<CardDataManagement> cardDataManagement = GetLevelManagement(chapterNumber, levelNumber).getCardData;//杩斿洖鍗＄墝鍐呭
        foreach (CardDataManagement card in cardDataManagement)
        {
            if (card.getRound <= endStepCount)
            {
                cardDataList.Add(getCardData.Find(x => x.getCardType == card.getCardType));//娣诲姞鍗＄墝鍐呭
            }
        }
        return cardDataList;
    }



}

