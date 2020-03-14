using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automated_Backpack_Scraper
{
    class BackpackListing 
    {
        public double priceRefinedMetal;
        public string unusualEffect;

        public BackpackListing(double Price, string Effect)
        {
            priceRefinedMetal = Price;
            unusualEffect = Effect;
        }

        public int compareTo(BackpackListing comparedListing)
        {
            if (priceRefinedMetal > comparedListing.priceRefinedMetal)
            {
                return -1;
            }
            else if (priceRefinedMetal < comparedListing.priceRefinedMetal)
            {
                return 1;
            }
            return 0;
        }
    }
}
