namespace Auralsys.Audio
{
    public class Device
    {
        private readonly string _name;       
        public Device(int index, string name)
        {
            Index = index;
            _name = name;           
        }

        public int Index { get; }
        public override string ToString() => _name;
    }
}
