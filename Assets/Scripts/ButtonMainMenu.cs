using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonMainMenu : MonoBehaviour, IPointerEnterHandler
{
    private GameObject player;
    private void Start()
    {
        player = GameObject.Find("PlayerMainMenu");        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        player.transform.position = new Vector2(transform.position.x + 3.5f, transform.position.y - 0.3f);
    }
}
