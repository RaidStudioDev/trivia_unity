using System;
using UnityEngine;
 
// JSON Helper - The Unity based JSONUtility does not parse multiple json objects in an array.
// If your json output contains an array and its key name, either replace "users" or 
// Original Reference: https://gist.github.com/halzate93/77e2011123b6af2541074e2a9edd5fc0

public static class JsonHelper
{
	public static T[] FromJson<T>(string jsonArray)
	{
		jsonArray = WrapArray (jsonArray);
		return FromJsonWrapped<T> (jsonArray);
	}

	public static T[] FromJsonWrapped<T> (string jsonObject)
	{
		// Debug.Log ("jsonObject: " + jsonObject);
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonObject);
		return wrapper.items;
	}

	private static string WrapArray (string jsonArray)
	{
		return "{ \"items\": " + jsonArray + "}";
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.items = array;
		return JsonUtility.ToJson(wrapper);
	}

	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[Serializable]
	private class Wrapper<T>
	{
		public T[] items;
	}
}