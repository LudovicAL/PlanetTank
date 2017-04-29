//This static class stores information that is relevant application-wide.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDataAndTools {

	public static void Shuffle<T>(this IList<T> list) {
		int listSize = list.Count - 1;
		int n = 0;
		while (n <= listSize) {
			int k = Random.Range (0, listSize);
			T value = list[n];
			list[n] = list[k];
			list[k] = value;
			n++;
		}
	}
}