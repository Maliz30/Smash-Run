using UnityEngine;

public interface IHammerInputProvider
{
    Vector3 GetMovementInput();

    float GetHeightInput();

    bool GetAttackTrigger();
}