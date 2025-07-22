using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private Transform[] _openBox;
    [SerializeField] private Transform[] _closeBox;
    [SerializeField] private GameObject[] _go;
    
    private void Start()
    {
        SpawnGoalPrefabs();
        AnimOpenBox();
    }
    private void DrawOutLine(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(4f, -4f);
        }
    }
    private void AnimOpenBox()
    {
        float animSize = 1.2f;
        float animDuration = 0.7f;
        foreach(Transform box in _openBox)
        {
            if(box.childCount == 0)
            {
                continue;
            }

            GameObject child = box.GetChild(0).gameObject;
            child.transform.DOScale(Vector3.one * animSize,animDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1,LoopType.Yoyo);
            DrawOutLine(child);
        }
    }
    private void SpawnGoalPrefabs()
    {
        // Tüm kutuları tek listede topla
        List<Transform> allBoxes = new List<Transform>();
        allBoxes.AddRange(_openBox);
        allBoxes.AddRange(_closeBox);

        List<Transform> closedBoxList = new List<Transform>(_closeBox);

        // Rastgele pozisyonlar için karıştır
        Shuffle(allBoxes);

        int totalToSpawn = _go.Length * 2;
        if (allBoxes.Count < totalToSpawn)
        {
            Debug.LogWarning("boxes not enough!");
            return;
        }

        int index = 0;
        foreach (GameObject prefab in _go)
        {
            for (int i = 0; i < 2; i++)
            {
                Transform targetBox = allBoxes[index];
                GameObject newObj = Instantiate(prefab,targetBox);
                newObj.transform.localPosition = Vector3.zero;
                //newObj.transform.localScale = Vector3.one * 110f;

                if (closedBoxList.Contains(targetBox))
                {
                  AlphaAdjustable(newObj);
                }
                index++;
            }
        }
    }
    private void AlphaAdjustable(GameObject obj)
    {
        Image image = obj.GetComponent<Image>();
        float alphaValue = 0.5f;
        if (image != null)
        {
            Color color = image.color;
            color.a = alphaValue;
            image.color = color;    
        }
    }
    //Tuple shuffle 
    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
