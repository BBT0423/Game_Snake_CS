using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Directon
    {
        public readonly static Directon Left = new Directon(0, -1);
        public readonly static Directon Right = new Directon(0, 1);
        public readonly static Directon Up = new Directon(-1, 0);
        public readonly static Directon Down = new Directon(1, 0);

        public int RowOffset { get; }
        public int ColumnOffset { get; }

        private Directon(int rowOffset, int colOffset) 
        {
            RowOffset = rowOffset;
            ColumnOffset = colOffset;
        }

        public Directon Opposite()
        {
            return new Directon(-RowOffset,-ColumnOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Directon directon &&
                   RowOffset == directon.RowOffset &&
                   ColumnOffset == directon.ColumnOffset;
        }

        public override int GetHashCode()
        {
            int hashCode = 1487996884;
            hashCode = hashCode * -1521134295 + RowOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + ColumnOffset.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Directon left, Directon right)
        {
            return EqualityComparer<Directon>.Default.Equals(left, right);
        }

        public static bool operator !=(Directon left, Directon right)
        {
            return !(left == right);
        }
    }
}
