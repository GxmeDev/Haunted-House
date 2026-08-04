using UnityEngine.Events;

public static class GameEvents
{
    public static event UnityAction Jump;
    public static event UnityAction Caught;

    public static void RaiseJump()
    {
        Jump.Invoke();
    }

    public static void RaiseCaught()
    {
        Caught.Invoke();
    }
}
