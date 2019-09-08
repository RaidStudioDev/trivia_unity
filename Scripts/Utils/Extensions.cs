using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public static class Extensions {

private static System.Random rnd = new System.Random();


public static void ListShuffle<T>(this List<T> self, int numberOfTimesToShuffle = 1){
List<T> newList = new List<T>();
for (int i = 0; i < numberOfTimesToShuffle; i++){
while (self.Count > 0)
{
int index = rnd.Next(self.Count);
newList.Add(self[index]);
self.RemoveAt(index);
}
self.AddRange(newList);
newList.Clear();
        }
    }

public static void ShowCanvasGroup(this CanvasGroup self) {
    self.alpha = 1f;
    self.blocksRaycasts = true;
    }

public static void HideCanvasGroup(this CanvasGroup self) {
    self.alpha = 0f;
    self.blocksRaycasts = false;
    }

//public static void ButtonScaleAnim(this Animation self)
//    {
//        self.Play("ButtonScale", PlayMode.StopAll);
//    }


 //Quiz.instance.pointstext.GetComponent<Animation>().Play("PointsScale", PlayMode.StopAll);
}