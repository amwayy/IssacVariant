using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSelection : MonoBehaviour
{
    [SerializeField] private GameLibrary.Element element;
    [SerializeField] private Transform grassPropertyPrefab;
    [SerializeField] private Transform firePropertyPrefab;
    [SerializeField] private Transform waterPropertyPrefab;
    [SerializeField] private Transform lightPropertyPrefab;
    [SerializeField] private Transform darkPropertyPrefab;

    private Transform elementPropertyPrefab;

    private void Awake()
    {
        switch(element)
        {
            case GameLibrary.Element.Grass:
                elementPropertyPrefab = grassPropertyPrefab;
                break;
            case GameLibrary.Element.Fire:
                elementPropertyPrefab = firePropertyPrefab;
                break;
            case GameLibrary.Element.Water:
                elementPropertyPrefab = waterPropertyPrefab;
                break;
            case GameLibrary.Element.Light:
                elementPropertyPrefab = lightPropertyPrefab;
                break;
            case GameLibrary.Element.Dark:
                elementPropertyPrefab = darkPropertyPrefab;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            Player.Instance.SetElement(element);
            Instantiate(elementPropertyPrefab, player.transform);

            RoomManager.Instance.EnterNewRoom(RoomManager.RoomType.Regular);
        }
    }
}
