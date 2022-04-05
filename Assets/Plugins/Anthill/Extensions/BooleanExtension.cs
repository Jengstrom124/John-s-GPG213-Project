namespace Anthill.Extensions
{
	public static class BooleanExtension
	{
		private const string _true = "●";
		private const string _false = "○";

		public static string ToStr(this bool aValue)
		{
			return (aValue) ? _true : _false;
		}
	}
}