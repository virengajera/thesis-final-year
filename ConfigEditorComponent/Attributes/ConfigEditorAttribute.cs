using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditorComponent.Component
{
	public class ConfigEditorHidden : Attribute
	{

	}

	public class ConfigEditorCollapsable : Attribute {

		public int spacingTopBottom { get; set; } = 0;
		public ConfigEditorCollapsable()
		{
			spacingTopBottom = 0;
		}

		public ConfigEditorCollapsable(int px)
		{
			spacingTopBottom = px;
		}
	}
}
