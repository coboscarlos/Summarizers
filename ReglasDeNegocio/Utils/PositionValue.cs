using System;

namespace BusinessRules.Utils
{
    public class PositionValue : IComparable<PositionValue>
    {
        public int Position;
        public double Value;

        public PositionValue(int miId, double miFitness)
        {
            Position = miId;
            Value = miFitness;
        }

        public int CompareTo(PositionValue other)
        {
            return -1 * Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Position + " " + Value.ToString("#0.###");
        }
    }
}