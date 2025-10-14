using UnityEngine;

public class SpriteMinimapIcon : MonoBehaviour
{
    public SpriteRenderer spriteRenderer = null;
    public Vector2 targetSize = new Vector2(1f, 1f);
    public Vector3 fixedRotation;

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        transform.localScale = new Vector3(
            targetSize.x / spriteSize.x,
            targetSize.y / spriteSize.y,
            1f
        );
    }
}
