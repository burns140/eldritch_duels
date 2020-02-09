#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch.cards;

namespace eldritch {
    public static class Global
    {
        public static string username = "";
        public static int userID = 0;
        public static List<Card> userCards = new List<Card>();
        public static int usercredits = 0;
    }
}
