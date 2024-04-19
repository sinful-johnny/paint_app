using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclassW10
{
    internal interface IExportable
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public void Save(object element);
        public void Load();
    }
}
