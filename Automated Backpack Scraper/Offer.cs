using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automated_Backpack_Scraper
{
    class Offer
    {
        public double profitInRefined;
        public string unusualEffectAndName;
        public Offer(string unusualEffect, string unusualName, double profitInRefined)
        {
            this.profitInRefined = profitInRefined;
            unusualEffectAndName = unusualEffect + " " + unusualName;
        }

        public int compareTo(Offer otherOffer)
        {
            if (otherOffer.profitInRefined > profitInRefined)
            {
                return 1;
            }
            return -1;
        }

        public override bool Equals(object otherOffer)
        {
            if (otherOffer is Offer)
            {
                Offer currentOffer = (Offer)otherOffer;
                return (currentOffer.profitInRefined == profitInRefined) && (unusualEffectAndName.Equals(currentOffer.unusualEffectAndName));
            }
            return false;
        }

        public override int GetHashCode()
        {
            char[] currentName = unusualEffectAndName.ToCharArray();
            ulong currentTally = 0, currentMultiple = 0;
            for (int Index = 0; Index < currentName.Length; Index++)
            {
                currentMultiple = Convert.ToUInt64((currentName.Length - Index) * currentName[Index]);
                for (int j = currentName.Length - 1; j > Index; j--)
                {
                    currentMultiple *= currentMultiple;
                    currentMultiple %= (int.MaxValue / 2) - 1;
                }
                currentTally += currentMultiple;
                currentTally %= (int.MaxValue / 2) - 1;
            }
            return (int)(profitInRefined * 100) + (int)currentTally;
        }
    }
}
