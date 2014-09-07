using System.Collections.Generic;
using VBone.Data;

namespace VBone.Logic
{
    public static class MatrixDataGenerator
    {
        public static IEnumerable<TromboneNote> Generate()
        {
            var data = new List<TromboneNote>();

            for (int h = 7; h >= 0; h--)
            {
                for (int p = 0; p < 7; p++)
                {
                    data.Add(new TromboneNote((Position)p, (Harmonic)h));
                }
            }

            return data;
        }
    }
}
