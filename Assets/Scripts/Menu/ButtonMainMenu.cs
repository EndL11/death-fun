using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonMainMenu : MonoBehaviour, IPointerEnterHandler
{
    private GameObject _player;
    private void Start()
    {
        _player = GameObject.Find("PlayerMainMenu");        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _player.transform.position = new Vector2(transform.position.x + 3.5f, transform.position.y - 0.3f);
    }
}
