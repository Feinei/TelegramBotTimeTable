namespace TimeTBot
{
    public abstract class Command
    {
        public string Name { get; }

        public Command(string name)
        {
            Name = name;
        }

        public bool Contains(string message)
        {
            return message.StartsWith(this.Name);
        }

        public abstract string GetDescription();

        public abstract string Execute(string id, string message);
    }
}
