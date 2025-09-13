using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Items/Potion")]
public class Potion : ItemData
{
    [SerializeField] private int heal = 0;

    public int Heal => heal;

    public override void Use(GameObject user)
    {
        //aumentar la vida al jugador
    }
}
