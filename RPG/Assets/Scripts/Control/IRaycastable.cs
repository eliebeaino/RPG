namespace RPG.Control
{
    interface IRaycastable
    {
        CursorType GetCursorType();

        bool HandleRaycast(PlayerController callingController);
    }
}
