using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TimeJob.Models
{
    class ProcessModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool responding { get; set; }
        public long memory { get; set; }
        public string path { get; set; }
        public bool showPath { get { return this.path != null; } set { this.showPath = value; } }
        public BitmapSource bitmapSource { get; set; }
        public ProcessModel processModel { get { return this; } }
    }
}
