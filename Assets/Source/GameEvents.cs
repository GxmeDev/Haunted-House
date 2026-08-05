using Source.Puzzle;
using UnityEngine.Events;

public static class GameEvents
{
    public static event UnityAction Jump;
    public static event UnityAction Caught;
    public static event UnityAction FadeInComplete;
    public static event UnityAction FadeScreenReset;
    public static event UnityAction<KeySO> Unlock;

    public static void RaiseJump()
    {
        Jump.Invoke();
    }

    public static void RaiseCaught()
    {
        Caught.Invoke();
    }

    public static void RaiseFadeInComplete()
    {
        FadeInComplete.Invoke();
    }

    public static void RaiseFadeScreenReset()
    {
        FadeScreenReset.Invoke();
    }

    public static void RaiseUnlock(KeySO keyData)
    {
        Unlock.Invoke(keyData);
    }
}
