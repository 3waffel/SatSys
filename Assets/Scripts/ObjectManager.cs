using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private int _itemsToGenerate;

    public enum ObjectType
    {
        Satellite,
        Station,
    }

    void Start()
    {
        TestScrollView();
    }

    void TestScrollView()
    {
        for (int i = 0; i < _itemsToGenerate; i++)
        {
            var item_go = Instantiate(_itemPrefab);
            // do something with the instantiated item -- for instance
            item_go.GetComponentInChildren<TMP_Text>().text = "Item #" + i;
            // item_go.GetComponent<Image>().color = i % 2 == 0 ? Color.yellow : Color.cyan;
            //parent the item to the content container
            item_go.transform.SetParent(_contentContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;
        }
    }
}
