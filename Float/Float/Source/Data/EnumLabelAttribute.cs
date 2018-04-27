
using System;

namespace Float
{
	public class EnumLabelAttribute : Attribute
	{
		public string label;
		public new int[] order = new int[0];

		public EnumLabelAttribute(string label)
		{
			this.label = label;
		}

		public EnumLabelAttribute(string label, params int[] order)
		{
			this.label = label;
			this.order = order;
		}
	}


	public class EnumLanguageAttribute : Attribute
	{
		public int label;

		public EnumLanguageAttribute(int label)
		{
			this.label = label;
		}
	}

}