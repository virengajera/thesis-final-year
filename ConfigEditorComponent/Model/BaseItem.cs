namespace ConfigEditorComponent.Model
{
	public class BaseItem
	{
		public string? PropertyName { get; set; }

		public Type? Type { get; set; }

		public string? StringKindValue { get; set; }
		public int? IntKindValue { get; set; }

		public long? LongKindValue { get; set; }
		public double? DoubleKindValue { get; set; }

		public bool? BooleanKindValue { get; set; }

		public bool isNull { get; set; } = false;

		public bool isDisabled { get; set; } = false;


	}
}
