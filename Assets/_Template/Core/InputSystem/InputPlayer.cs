namespace _Template.Core.InputSystem
{
    public struct InputPlayer
    {
        public int Id { get; }

        public InputPlayer(int id, bool isPair)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"Player {Id}";
        }
    }
}