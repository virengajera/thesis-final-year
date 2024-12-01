using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditorComponent.Model
{
    internal class ListFormAddElement : BaseItem
    {
        public string? PropertyName { get; set; }
        public object? NewElement { get; set; }
    }
}
