using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_SpeedPotion", menuName = "Items/Speed Potion")]
public class SpeedPotion : ItemData
{
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float duration = 5f;

    public float SpeedMultiplier => speedMultiplier;
    public float Duration => duration;

    public override void Use(GameObject user)
    {
        PlayerController playerController = user.GetComponent<PlayerController>();
        if (playerController != null)
        {
            PlayUseAudio(user.transform.position);
            playerController.StartCoroutine(ApplySpeedBoost(playerController));
        }
    }

    private IEnumerator ApplySpeedBoost(PlayerController player)
    {
        player.ModifySpeed(speedMultiplier);
        player.EnableSpeedEffect();
        yield return new WaitForSeconds(duration);
        player.RestoreSpeed();
        player.DisableSpeedEffect();
    }
}
