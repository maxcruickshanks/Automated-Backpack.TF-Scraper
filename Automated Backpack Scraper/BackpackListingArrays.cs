namespace Automated_Backpack_Scraper
{
    class BackpackListingArrays
    {
        public BackpackListing[] buyOrders, sellOrders;
        public BackpackListingArrays(BackpackListing[] buyOrders, BackpackListing[] sellOrders)
        {
            this.buyOrders = buyOrders;
            this.sellOrders = sellOrders;
        }
    }
}