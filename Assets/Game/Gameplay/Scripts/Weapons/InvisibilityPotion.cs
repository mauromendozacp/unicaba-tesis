using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_InvisibilityPotion", menuName = "Items/Invisibility Potion")]
public class InvisibilityPotion : ItemData
{
    [SerializeField] private float duration = 5f;

    public float Duration => duration;

    public override void Use(GameObject user)
    {
        PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            PlayUseAudio(user.transform.position);
            playerHealth.StartCoroutine(ApplyInvincibility(playerHealth));
        }
    }

    private IEnumerator ApplyInvincibility(PlayerHealth playerHealth)
    {
        playerHealth.SetInvincibility(true);
        yield return new WaitForSeconds(duration);
        playerHealth.SetInvincibility(false);
    }
}
