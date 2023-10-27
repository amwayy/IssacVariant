using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject chestGameObejct;
    [SerializeField] private GameObject chestOpenGameObject;
    [SerializeField] private Transform lootWindowPrefab;
    [SerializeField] private float backpackWindowUIVisualOffset = 160f;
    
    private int layerAbovePlayer = 4;
    private int layerBelowPlayer = 2;
    private bool isOpen = false;

    private void Awake()
    {
        chestOpenGameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player) && !isOpen)
        {
            OpenChest();
            isOpen = true;
        }
    }

    private void OpenChest()
    {
        chestGameObejct.SetActive(false);
        chestOpenGameObject.SetActive(true);

        Backpack.Instance.OpenClose();
        Backpack.Instance.GetBackpackWindowTransform().GetComponent<BackpackWindowUI>().SetVisualOffset(Vector3.down * backpackWindowUIVisualOffset);
        Instantiate(lootWindowPrefab, GameLibrary.Instance.GetCanvasTransform());
    }

     private void UpdateLayer()
    {
        if (Player.Instance.transform.position.y > transform.position.y)
        {
            chestGameObejct.GetComponent<SpriteRenderer>().sortingOrder = layerAbovePlayer;
            chestOpenGameObject.GetComponent<SpriteRenderer>().sortingOrder = layerAbovePlayer;
        } else
        {
            chestGameObejct.GetComponent<SpriteRenderer>().sortingOrder = layerBelowPlayer;
            chestOpenGameObject.GetComponent<SpriteRenderer>().sortingOrder = layerBelowPlayer;
        }
    }
}
