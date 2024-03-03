using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject jsonParserPrefab;
    [SerializeField] private GameObject levelSelectItemPrefab;
    public static int maxLevels;


    // Start is called before the first frame update
    void Start()
    {
        GameObject jsonParser = Instantiate(jsonParserPrefab, new Vector3(0, 0), Quaternion.identity);
        jsonParser.GetComponent<JsonParser>().readFile("levelDescriptor");
        maxLevels = jsonParser.GetComponent<JsonParser>().levelData.Count;

        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        for (int i = 0; i < maxLevels; i++)
        {
            GameObject levelSelectItem = Instantiate(levelSelectItemPrefab, new Vector3(0, 0), Quaternion.identity);
            levelSelectItem.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            levelSelectItem.transform.SetParent(gridLayoutGroup.transform, false);

            int levelStars = 0;
            if (i > 0) { levelStars = PlayerPrefs.GetInt("Level_" + i.ToString(), -1); }
            else { levelStars = PlayerPrefs.GetInt("Level_" + i.ToString(), 0); }

            levelSelectItem.GetComponent<LevelSelectItem>().setLevelSelectItem(i, levelStars);


            //if(i == 0) levelSelectItem.GetComponent<LevelSelectItem>().isLocked = false;

            //levelSelectItem.transform.SetParent(gridLayoutGroup.transform, false);
            //levelSelectItem.GetComponent<RectTransform>().localScale = Vector3.one;
            //levelSelectItem.GetComponent<LevelSelectItem>().OnRectTransformDimensionsChange();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

}
