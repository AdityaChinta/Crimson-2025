using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider healthSlider;
    public PlayerControl player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSlider.maxValue = player.maxHealth;
        healthSlider.value = player.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = player.GetHealth();
    }
}
