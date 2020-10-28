namespace Auralsys.Audio
{
    public class Device
    {
        string _name;       
        public Device(int index, string name, bool isLoopback)
        {
            Index = index;
            _name = name;
            IsLoopback = isLoopback;           
        }

        public int Index { get; }
        public bool IsLoopback { get; }
        public override string ToString() => _name;
    }
}
