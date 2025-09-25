using UnityEngine;

public interface IRevivable
{
  // Indica si la entidad está en un estado que puede ser revivido
  bool IsRevivable { get; }

  // Realiza la acción de revivir, por ejemplo, restaurar vida y habilitar el control
  void Revive(float reviveAmount);

  // Devuelve la posición de la entidad para verificar el rango
  Vector3 GetPosition();
}