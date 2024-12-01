using ConfigEditorComponent.Component;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConfigEditorMain.Models
{
	public class Settings
	{
		[Description("Tooltip demo for the property")]
		public string NormalStringType { get; set; } = string.Empty;
        public string? NullableStringType { get; set; }
		
		public int NormalIntType { get; set; }

        [Description("jfsfsfsdfksfs")]
		public int? NullableIntType {  get; set; }
		public double NormalDoubleType { get; set; }
        public double? NullableDoubleType { get; set; }

        public bool NormalBoolType { get; set; }
        public bool? NullableBoolType { get; set; }

        public List<int>? ListIntType { get; set; }
        public Dictionary<string, string> NormalStringDictinary { get; set; } = null!;
        //[ConfigEditorCollapsable]
        [ConfigEditorHidden]
        public ClassType? ClassType {  get; set; }



    }

    public class ClassType
    {
        public string AdminURL { get; set; } = null!;
		public Dictionary<string, int> NormalIntDictinary { get; set; } = null!;
		public List<string>? ListStringType { get; set; }
        public NestedClass? NestedClassLevel_1 { get; set; }

    }

	public class NestedClass
	{
		
		public int SomeIntValue { get; set; }

		public string? SomeStringValue { get; set; }

        public Dictionary<string, DbServer>? StructureDictionary { get; set; }

    }
    public class DbServer
	{ 
		public string Host { get; set; } = string.Empty;
		public int Port { get; set; }
		public int? BackupEvery {  get; set; }
		public string Conf_File { get; set; } = string.Empty;
		public bool? CacheResult { get; set; }
		public bool ssl { get; set; } = true;
		public int idleTimeOut { get; set; } = 1000;


	}
}
