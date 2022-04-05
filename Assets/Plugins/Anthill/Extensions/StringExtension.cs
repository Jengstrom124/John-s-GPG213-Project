namespace Anthill.Extensions
{
	using System.Collections.Generic;

	public static class StringExtension
	{
		public static string RemoveSpaces(this string aValue)
		{
			char[] characters = aValue.ToCharArray();
			List<char> nonBlankChars = new List<char>();

			char blank = ' ';
			int numChars = characters.Length;
			for (int i = 0; i < numChars; i++)
			{
				if (characters[i] != blank)
				{
					nonBlankChars.Add(characters[i]);
				}
			}

			return new string(nonBlankChars.ToArray());
		}
	}
}