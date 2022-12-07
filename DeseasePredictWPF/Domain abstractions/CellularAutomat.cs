using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeseasePredictWPF.Interfaces
{
    public abstract class CellularAutomat
    {
        public virtual int Height { get; set; }

        public abstract int Width { get; set; }

        public virtual int Length { get; set; }

        public abstract Cell this[int i] { get; }
        public abstract Cell this[int i, int j] { get; }

        public abstract Cell this[int i, int j, int k] { get; }

        public abstract void NextStep();

        public abstract Bitmap MakePicture(int _PictureHeight, int _PictureWidth);

        public abstract List<DailyStatistics> GetStatistics();

    }
}
