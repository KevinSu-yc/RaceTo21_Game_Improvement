using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Card
    {
        public string id;
        public string displayName;

        public Card(string shordCardName, string longCardName)
        {
            id = shordCardName;
            displayName = longCardName;
        }

    }
}
