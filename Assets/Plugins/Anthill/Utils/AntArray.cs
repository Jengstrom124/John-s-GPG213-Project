namespace Anthill.Utils
{
	/// <summary>
	/// Helper that simplfy working with arrays.
	/// </summary>
	public static class AntArray
	{
		/// <summary>
		/// Determines if a value is in an array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <param name="aValue">Value.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Returns true if value existing in the Array.</returns>
		public static bool Contains<T>(T[] aSource, T aValue)
		{
			int index = System.Array.FindIndex(aSource, x => System.Object.ReferenceEquals(x, aValue));
			return (index >= 0 && index < aSource.Length);
		}

		/// <summary>
		/// Adds new value to the Array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <param name="aValue">Value.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		public static void Add<T>(ref T[] aSource, T aValue)
		{
			var newArray = new T[aSource.Length + 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				newArray[i] = aSource[i];
			}

			newArray[newArray.Length - 1] = aValue;
			aSource = newArray;
		}

		/// <summary>
		/// Removes value from the Array by index.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <param name="aDelIndex">Index.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		public static void RemoveAt<T>(ref T[] aSource, int aDelIndex)
		{
			int curIndex = 0;
			var newArray = new T[aSource.Length - 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				if (i != aDelIndex)
				{
					newArray[curIndex] = aSource[i];
					curIndex++;
				}
			}

			aSource = newArray;
		}

		/// <summary>
		/// Gets random value from the Array.
		/// </summary>
		/// <param name="aSource">Array</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Random value of the Array.</returns>
		public static T GetRandom<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				result = aSource[AntMath.RandomRangeInt(0, aSource.Length - 1)];
			}
			return result;
		}

		/// <summary>
		/// Pops random value from the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Random value of the Array.</returns>
		public static T PopRandom<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				int i = AntMath.RandomRangeInt(0, aSource.Length - 1);
				result = aSource[i];
				RemoveAt<T>(ref aSource, i);
			}
			return result;
		}

		/// <summary>
		/// Gets first value from the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>First value.</returns>
		public static T First<T>(ref T[] aSource)
		{
			return (aSource.Length > 0) ? aSource[0] : default(T);
		}

		/// <summary>
		/// Gets last value from the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Last value.</returns>
		public static T Last<T>(ref T[] aSource)
		{
			return (aSource.Length > 0) ? aSource[aSource.Length - 1] : default(T);
		}

		/// <summary>
		/// Pops first value from the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>First value.</returns>
		public static T PopFirst<T>(this T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				result = aSource[0];
				RemoveAt<T>(ref aSource, 0);
			}
			return result;
		}

		/// <summary>
		/// Pops last value from the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Last value.</returns>
		public static T PopLast<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				int i = aSource.Length - 1;
				result = aSource[i];
				RemoveAt<T>(ref aSource, i);
			}
			return result;
		}

		/// <summary>
		/// Pops value from the array by index.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <param name="aIndex">Index.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Value from the array.</returns>
		public static T Pop<T>(ref T[] aSource, int aIndex)
		{
			T result = default(T);
			if (aIndex >= 0 && aIndex < aSource.Length)
			{
				result = aSource[aIndex];
				RemoveAt<T>(ref aSource, aIndex);
			}
			return result;
		}

		/// <summary>
		/// Creates copy of the array.
		/// </summary>
		/// <param name="aSource">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		/// <returns>Clone of the Array.</returns>
		public static T[] Clone<T>(ref T[] aSource)
		{
			var newArray = new T[aSource.Length];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				newArray[i] = aSource[i];
			}
			return newArray;
		}

		/// <summary>
		/// Shuffle all values in the array.
		/// </summary>
		/// <param name="aList">Array.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		public static void Shuffle<T>(ref T[] aList)
		{
			T temp;
			int newPos;
			int index = aList.Length;
			while (index > 1)
			{
				index--;
				newPos = AntMath.RandomRangeInt(0, index);
				temp = aList[newPos];
				aList[newPos] = aList[index];
				aList[index] = temp;
			}
		}

		/// <summary>
		/// Swap values in the arrau.
		/// </summary>
		/// <param name="aList">Array.</param>
		/// <param name="aIndexA">First index.</param>
		/// <param name="aIndexB">Second index.</param>
		/// <typeparam name="T">Type of Array.</typeparam>
		public static void Swap<T>(ref T[] aList, int aIndexA, int aIndexB)
		{
			if (aIndexA < 0 || aIndexB < 0 ||
				aIndexA == aIndexB ||
				aIndexA >= aList.Length || aIndexB >= aList.Length)
			{
				return;
			}

			T temp = aList[aIndexA];
			aList[aIndexA] = aList[aIndexB];
			aList[aIndexB] = temp;
		}
	}
}