using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automated_Backpack_Scraper
{
    public partial class mainForm : Form
    {
        const string backpackAPIKey = "__USE_YOUR_API_KEY_HERE__";
        //TO DO: Create an automatic method to grab the refined metal cost per key.
	    const double refinedPerKey = 54.33;
        //Do NOT fiddle with this number (i.e., it is the maximum, as stated by BP.TF: 120 requests per minute); it will cause you to be temporarily banned from making requests to the server.
        const int backpackRateLimit = 500;

        static BackpackListing[] buyOrders; //We will need to "sell" it.
        static BackpackListing[] sellOrders; //We will need to "buy" it.
        static List<Offer> Offers = new List<Offer>();
        static HashSet<Offer> previousDisplayedOffers = new HashSet<Offer>();

        public mainForm()
        {
            InitializeComponent();
        }

        BackpackListingArrays getListings(string hatName)
        {
            BackpackListingArrays finalListings;
            List<BackpackListing> currentBuyListings = new List<BackpackListing>();
            List<BackpackListing> currentSellListings = new List<BackpackListing>();
            int totalCycles = -1;
            int currentCycle = 0;
            string urlStart = "https://backpack.tf/api/classifieds/search/v1?";
            string Key = "key=" + backpackAPIKey;
            string Quality = "&quality=5";
            string pageSize = "&page_size=30";
            string Slot = "&slot=misc";
            string Folding = "&fold=0";
            //No impact on results
            //string Intent = (buyOrder) ? "&intent=sell" : "&intent=buy";
            string Item = "&item=" + hatName;
            string Page;
            long previousTime;
		
		    while (totalCycles >= currentCycle || totalCycles == -1)
		    {
			    Page = "&page=" + currentCycle;
			    string buildURL = urlStart + Key + Quality + Item + pageSize + Slot + Page + Folding;
                string HTML = downloadWebPage(buildURL);
                previousTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(); ;
			    if (int.Parse(HTML.Split(new[] { "\"total\":" }, StringSplitOptions.None)[1].Split(',')[0]) == 0)
			    {
                    //Don't return anything!
				    throw new Exception("No listings found!");
                }


                string[] splitBuyListings = HTML.Split(new[] { "\"sell\":" }, StringSplitOptions.None)[0].Split(new[] { "\"item\":" }, StringSplitOptions.None);
                string[] splitSellListings = HTML.Split(new[] { "\"sell\":" }, StringSplitOptions.None)[1].Split(new[] { "\"item\":" }, StringSplitOptions.None);

                if (totalCycles == -1)
			    {
				    //30.0: Page limit; 0.5: Round-up properly.
				    totalCycles = (int) Math.Round((int.Parse(HTML.Split(new[] { "\"total\":" }, StringSplitOptions.None)[1].Split(',')[0]) / 30.0) + 0.5);
			    }
			    for (int i = 1; i < splitBuyListings.Length; i++)
			    {
				    string currentCurrency = splitBuyListings[i].Split(new[] { "\"currencies\":" }, StringSplitOptions.None)[1];
                    double currentMetal = 0;
                    double currentKeys = 0;
				    if (currentCurrency.Contains("keys"))
				    {
					    currentKeys = Double.Parse(currentCurrency.Split('}')[0].Split(new[] { "\"keys\":" }, StringSplitOptions.None)[1]);
					    if (currentCurrency.Contains("metal"))
					    {
						    currentMetal = Double.Parse(currentCurrency.Split(',')[0].Split(new[] { "\"metal\":" }, StringSplitOptions.None)[1]);
					    }
				    }
				    else
				    {
					    currentMetal = Double.Parse(currentCurrency.Split('}')[0].Split(new[] { "\"metal\":" }, StringSplitOptions.None)[1]);
				    }
				    string effectName = splitBuyListings[i].Split(new[] { "\"name\":\"" }, StringSplitOptions.None)[1].Split(new[] { hatName.Replace("%20", " ")}, StringSplitOptions.None)[0].Replace(" ", "");
				    if (effectName.Equals("Unusual"))
				    {
					    effectName = "Any";
					    currentCycle = totalCycles;
				    }
				    BackpackListing currentListing = new BackpackListing(currentKeys * refinedPerKey + currentMetal, effectName);
                    currentBuyListings.Add(currentListing);
			    }
                for (int i = 1; i < splitSellListings.Length; i++)
                {
                    string currentCurrency = splitSellListings[i].Split(new[] { "\"currencies\":" }, StringSplitOptions.None)[1];
                    double currentMetal = 0;
                    double currentKeys = 0;
                    if (currentCurrency.Contains("keys"))
                    {
                        currentKeys = Double.Parse(currentCurrency.Split('}')[0].Split(new[] { "\"keys\":" }, StringSplitOptions.None)[1]);
                        if (currentCurrency.Contains("metal"))
                        {
                            currentMetal = Double.Parse(currentCurrency.Split(',')[0].Split(new[] { "\"metal\":" }, StringSplitOptions.None)[1]);
                        }
                    }
                    else
                    {
                        currentMetal = Double.Parse(currentCurrency.Split('}')[0].Split(new[] { "\"metal\":" }, StringSplitOptions.None)[1]);
                    }
                    string effectName = splitSellListings[i].Split(new[] { "\"name\":\"" }, StringSplitOptions.None)[1].Split(new[] {hatName.Replace("%20", " ")}, StringSplitOptions.None)[0].Replace(" ", "");
                    if (effectName.Equals("Unusual"))
                    {
                        effectName = "Any";
                        currentCycle = totalCycles;
                    }
                    BackpackListing currentListing = new BackpackListing(currentKeys * refinedPerKey + currentMetal, effectName);
                    currentSellListings.Add(currentListing);
                }

                currentCycle++;
		    }
            int Index = 0;
            BackpackListing[] buyOrders = new BackpackListing[currentBuyListings.Count], sellOrders = new BackpackListing[currentSellListings.Count];
		    foreach (BackpackListing Listing in currentBuyListings)
		    {
			    buyOrders[Index] = Listing;
                Index++;
		    }
            Index = 0;
            foreach (BackpackListing Listing in currentSellListings)
            {
                sellOrders[Index] = Listing;
                Index++;
            }
            finalListings = new BackpackListingArrays(buyOrders, sellOrders);
            return finalListings;
	    }

        public void findOffers()
        {
            List<string> Hats = hatNames();
            BackpackListingArrays currentOrders;
            Offer currentOffer;
            while (true)
            {
                foreach (string Hat in Hats)
                {
                    try
                    {
                        currentOrders = getListings(Hat);
                        buyOrders = currentOrders.buyOrders;
                        sellOrders = currentOrders.sellOrders;
                        Array.Sort<BackpackListing>(sellOrders, delegate (BackpackListing Found, BackpackListing Given) { return (int)(Given.priceRefinedMetal - Found.priceRefinedMetal); });
                        Array.Sort<BackpackListing>(buyOrders, delegate (BackpackListing Found, BackpackListing Given) { return (int)(Given.priceRefinedMetal - Found.priceRefinedMetal); });
                        if (sellOrders.Length > 0 && buyOrders.Length > 0)
                        {
                            //Quick profit check.
                            if (buyOrders[0].priceRefinedMetal - sellOrders[sellOrders.Length - 1].priceRefinedMetal > 0)
                            {
                                foreach (BackpackListing sellOrder in sellOrders)
                                {
                                    foreach (BackpackListing buyOrder in buyOrders)
                                    {
                                        currentOffer = new Offer(sellOrder.unusualEffect, Hat, buyOrder.priceRefinedMetal - sellOrder.priceRefinedMetal);
                                        if (!previousDisplayedOffers.Contains(currentOffer) && buyOrder.priceRefinedMetal - sellOrder.priceRefinedMetal > 0 && (sellOrder.unusualEffect.Equals(buyOrder.unusualEffect) || buyOrder.unusualEffect.Equals("Any")))
                                        {
                                            previousDisplayedOffers.Add(currentOffer);
                                            Offers.Add(currentOffer);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //Update offer list.
                        if (queueButton.Enabled == false && Offers.Count > 0)
                        {
                            queueButton.Invoke((MethodInvoker)delegate
                           {
                               queueButton.Enabled = true;
                           });
                            itemDisplay.Invoke((MethodInvoker)delegate
                            {
                                itemDisplay.Text = "Current item: " + Offers.First().unusualEffectAndName.Replace("%20", " ");
                            });
                            System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav");
                            soundPlayer.Play();
                        }
                    }
                    catch (Exception e){}
                }
            }
        }

        public static string downloadWebPage(string URL)
        {
            WebClient Client = new WebClient();
            Client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)"); //Make the server think we're a Mozilla browser.
            Stream Data = Client.OpenRead(URL);
            StreamReader reader = new StreamReader(Data);
            return reader.ReadToEnd();
        }

        public static List<string> hatNames()
        {
            //Sloppy method for storing the https://backpack.tf/unusuals HTML source.
            string[] unusualList = (new HTMLFile().getFile()).Split(new[] { "id='unusual-pricelist'>" }, StringSplitOptions.None)[1].Split(new[] { "<li title=\"Unusual " }, StringSplitOptions.None);
            List<string> hatList = new List<string>();
            string hatName;
            foreach (string Unusual in unusualList)
            {
                hatName = Unusual.Split(new[] { "\" class=" }, StringSplitOptions.None)[0];
                if (Unusual.Length > 0)
                {
                    hatList.Add(Unusual.Split(new[] { "<a href=\"/unusual/"}, StringSplitOptions.None)[1].Split(new[] { "\">" }, StringSplitOptions.None)[0]);
                }
            }
            return hatList;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            Thread grabOffers = new Thread(new ThreadStart(findOffers));
            grabOffers.Start();
        }

        private void queueButton_Click(object sender, EventArgs e)
        {
            Offer currentOffer = Offers.First();
            Offers.RemoveAt(0);
            if (Offers.Count > 0)
            {
                Offers.RemoveAt(0);
            }

            if (Offers.Count > 0)
            {
                itemDisplay.Text = "Current item: " + Offers.First().unusualEffectAndName;
            }
            else
            {
                queueButton.Enabled = false;
                itemDisplay.Text = "Current item: N/A";
            }
            System.Diagnostics.Process.Start("https://backpack.tf/unusual/" + currentOffer.unusualEffectAndName.Split(' ')[1]);
        }
    }
}
