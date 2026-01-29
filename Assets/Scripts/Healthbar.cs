using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] RectTransform foreground;
    Health playerHealth;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent < Health >();
    }

    private void Update()
    {
        foreground.localScale = new Vector3(playerHealth.GetHealthPercentage(), 1f, 1f);
    }

}
