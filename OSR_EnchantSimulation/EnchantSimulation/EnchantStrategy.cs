using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace EnchantSimulation
{
    public class EnchantStrategy
    {
        public struct Trial
        {
            public bool Successful;
            public int UsedEnchantCards;
            public int Used3Boosters;
            public int Attempts;            
            public int Used5Boosters;
            public int Used8Boosters;
            public int UsedBEProts;
            public int UsedEProts;
            public int UsedHProts;
            public int UsedBooks;
            public int UsedItems; // for weapon/armor raging
            public int UsedSpCards;
        }

        public class TrialSignature : IComparable
        {
            public int UsedEnchantCards;
            public int Used3Boosters;
            public int Attempts;            
            public int Used5Boosters;
            public int Used8Boosters;
            public int UsedBEProts;
            public int UsedEProts;
            public int UsedHProts;
            public int UsedBooks;
            public int Frequency;
            public int UsedItems;
            public int UsedSpCards;

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;

                TrialSignature other = obj as TrialSignature;
                if (other == null)
                {
                    throw new ArgumentException("Object is not a TrialSignature!");
                }
                else
                {
                    int x = this.UsedEnchantCards.CompareTo(other.UsedEnchantCards);
                    if (x == 0)
                        x = this.Used3Boosters.CompareTo(other.Used3Boosters);
                    if (x == 0)
                        x = this.UsedHProts.CompareTo(other.UsedHProts);
                    if (x == 0)
                        x = this.UsedEProts.CompareTo(other.UsedEProts);
                    if (x == 0)
                        x = this.Used8Boosters.CompareTo(other.Used8Boosters);
                    if (x == 0)
                        x = this.Used5Boosters.CompareTo(other.Used5Boosters);
                    if (x == 0)
                        x = this.UsedBEProts.CompareTo(other.UsedBEProts);
                    if (x == 0)
                        x = this.UsedBooks.CompareTo(other.UsedBooks);
                    if (x == 0)
                        x = this.UsedItems.CompareTo(other.UsedItems);
                    if (x == 0)
                        x = this.UsedSpCards.CompareTo(other.UsedSpCards);
                    return x;
                }                    
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int StartingLevel = 0;
        public int TargetLevel = 12;
        public int Simulations = 100000;
        public int MaxSimulationLength = 1000000;
        public EnchantLevel.ItemType ItemType =  EnchantLevel.ItemType.WeaponArmor;
        public List<EnchantLevel> Levels;

        private List<Trial> Trials;
        private List<TrialSignature> Statistics;

        private Trial Test(Random r, double lv1Prob)
        {
            int i = 0, level = StartingLevel, boost5count = 0, boost8count = 0, eprotcount = 0, hprotcount = 0, cardcount = 0, boost3count = 0, bookcount = 0, beprotcount = 0, itemcount = 1, spcardcount = 0;

            for (; i < MaxSimulationLength; i++)
            {
                EnchantLevel el = Levels[level];

                if (el.Use3Booster)
                    boost3count++;

                if (el.Use5Booster)
                    boost5count++;

                if (el.Use8Booster)
                    boost8count++;

                if (el.UseBEProt)
                    beprotcount++;

                if (el.UseEProt)
                    eprotcount++;

                if (el.UseHProt)
                    hprotcount++;
                

                // Assume e10+ = hypers only. CHANGED: Check if the item is going to level 1 legend first. If it is, books are used. If not, cards are used.
                // There are no hyper cards in OSR!

                double prob;

                if (level == 0 && (ItemType == EnchantLevel.ItemType.LegendArmor || ItemType == EnchantLevel.ItemType.LegendWeapon))
                {
                    bookcount++;
                    prob = lv1Prob;
                }

                else //if (level < 10)
                {
                    if (el.UseSpCard && (ItemType == EnchantLevel.ItemType.WeaponArmor)) // if mixed enchants are used; special cards are energy/shield cards and speed cards in OSR
                    {
                        spcardcount++;
                    }

                    else { // normal enchant cards
                        cardcount++;
                    }

                    prob = el.GetDerivedProbability(level, ItemType);
                }
                //else
                //    boost3count++;                

                if (r.NextDouble() < prob)
                {
                    // success, increment
                    level++;

                    if (level == TargetLevel)
                        break;
                }
                else
                {
                    // Can't use eprots on attachments
                    if (ItemType == EnchantLevel.ItemType.LegendArmor || ItemType == EnchantLevel.ItemType.LegendWeapon)
                    {
                        level = 0;
                        itemcount++;
                    }
                    else
                    {
                        if (el.UseBEProt)
                        {
                            // failure; resets to level 1 for basic eprots
                            level = 1;
                        }

                        else if (el.UseEProt)
                        {
                            // failure, reset 5
                            level = 5;
                        }
                        else if (el.UseHProt)
                        {
                            // Reset to level 10 for hprots
                            level = 10;
                        }

                        else { 
                            level = 0;
                            itemcount++;
                        }
                    }
                }
            }

            return new Trial { Attempts = i + 1, Successful = (level == TargetLevel), UsedEnchantCards = cardcount, Used3Boosters = boost3count,
                                Used5Boosters = boost5count, Used8Boosters = boost8count, UsedBEProts = beprotcount, UsedEProts = eprotcount, UsedHProts = hprotcount, UsedBooks = bookcount, UsedItems = itemcount, UsedSpCards = spcardcount };
        }

        private List<Trial> RunSimulation(double lv1Prob)
        {
            List<Trial> ret = new List<Trial>();
            Random r = new Random();

            for (int i = 0; i < Simulations; i++)
            {
                ret.Add(Test(r, lv1Prob));
            }

            return ret;
        }

        public static double CalculateCost(double cards, double cardcost, double boost3, double boost3cost,
                                    double boost15, double boost15cost, double boost25, double boost25cost,
                                    double eprot, double eprotcost, double hprot, double hprotcost, double beprot, double beprotcost, double book, double bookcost, double item, double itemcost, double spcards, double spcardcost)
        {
            return (cards * cardcost) + (boost3 * boost3cost) + (boost15 * boost15cost) + (boost25 * boost25cost) +
                    (eprot * eprotcost) + (hprot * hprotcost) + (beprot * beprotcost) + (book * bookcost) + (item * itemcost) + (spcards * spcardcost);
            //return boost15 * 2000 + boost25 * 45000 + eprot * 3000 + hprot * 88000;
        }

        public void CalculateFrequencies(double lv1Prob)
        {
            Trials = RunSimulation(lv1Prob);

            Statistics = Trials.GroupBy(x => new { x.Attempts, x.UsedEnchantCards, x.Used3Boosters, x.Used5Boosters, x.Used8Boosters, x.UsedBEProts, x.UsedEProts, x.UsedHProts, x.UsedBooks, x.UsedItems, x.UsedSpCards })
                            .Select(x => new TrialSignature { Attempts = x.Key.Attempts, UsedEnchantCards = x.Key.UsedEnchantCards, Used3Boosters = x.Key.Used3Boosters,
                                                                Used5Boosters = x.Key.Used5Boosters, Used8Boosters = x.Key.Used8Boosters, UsedBEProts = x.Key.UsedBEProts,
                                                                UsedEProts = x.Key.UsedEProts, UsedHProts = x.Key.UsedHProts, UsedBooks = x.Key.UsedBooks, UsedItems = x.Key.UsedItems, UsedSpCards = x.Key.UsedSpCards, Frequency = x.Count() })
                            .OrderBy(x => x).ToList();
        }

        public string GenerateReport(double cardcost, double boost3cost, double boost5cost, double boost8cost, double beprotcost, double eprotcost, double hprotcost, double bookcost, double itemcost, double spcardcost)
        {            
            double avgec = Trials.Average(x => x.UsedEnchantCards);
            double avg3 = Trials.Average(x => x.Used3Boosters);
            double avg5 = Trials.Average(x => x.Used5Boosters);
            double avg8 = Trials.Average(x => x.Used8Boosters);
            double avgbe = Trials.Average(x => x.UsedBEProts);
            double avge = Trials.Average(x => x.UsedEProts);
            double avgh = Trials.Average(x => x.UsedHProts);
            double avgb = Trials.Average(x => x.UsedBooks);
            double avgi = Trials.Average(x => x.UsedItems);
            double avgsc = Trials.Average(x => x.UsedSpCards);

            double pct5 = -1, pct50 = -1, pct95 = -1, avgCost = -1;
            string formatted_pct5, formatted_pct50, formatted_pct95, formatted_avgCost;
            int cumulativeFrequency = 0;

            foreach(TrialSignature stat in Statistics)
            {
                cumulativeFrequency += stat.Frequency;

                if(cumulativeFrequency >= Math.Ceiling(Trials.Count * 0.05) && pct5 == -1)
                {
                    pct5 = CalculateCost(stat.UsedEnchantCards, cardcost, stat.Used3Boosters, boost3cost, stat.Used5Boosters, boost5cost, stat.Used8Boosters, boost8cost, stat.UsedEProts, eprotcost, stat.UsedHProts, hprotcost, stat.UsedBEProts, beprotcost, stat.UsedBooks, bookcost, stat.UsedItems, itemcost, stat.UsedSpCards, spcardcost);
                }
                else if(cumulativeFrequency >= Math.Ceiling(Trials.Count * 0.5) && pct50 == -1)
                {
                    pct50 = CalculateCost(stat.UsedEnchantCards, cardcost, stat.Used3Boosters, boost3cost, stat.Used5Boosters, boost5cost, stat.Used8Boosters, boost8cost, stat.UsedEProts, eprotcost, stat.UsedHProts, hprotcost, stat.UsedBEProts, beprotcost, stat.UsedBooks, bookcost, stat.UsedItems, itemcost, stat.UsedSpCards, spcardcost);
                }
                else if (cumulativeFrequency >= Math.Ceiling(Trials.Count * 0.95) && pct95 == -1)
                {
                    pct95 = CalculateCost(stat.UsedEnchantCards, cardcost, stat.Used3Boosters, boost3cost, stat.Used5Boosters, boost5cost, stat.Used8Boosters, boost8cost, stat.UsedEProts, eprotcost, stat.UsedHProts, hprotcost, stat.UsedBEProts, beprotcost, stat.UsedBooks, bookcost, stat.UsedItems, itemcost, stat.UsedSpCards, spcardcost);
                }
            }

            avgCost = CalculateCost(avgec, cardcost, avg3, boost3cost, avg5, boost5cost, avg8, boost8cost, avge, eprotcost, avgh, hprotcost, avgbe, beprotcost, avgb, bookcost, avgi, itemcost, avgsc, spcardcost);

            formatted_pct5 = pct5.ToString("N");
            formatted_pct50 = pct50.ToString("N");
            formatted_pct95 = pct95.ToString("N");
            formatted_avgCost = avgCost.ToString("N");

            return string.Format(@"Average Enchant Cards used = {0}, Average Special Enchant Cards used = {13},
Average 3% used = {1}, Average 5% used = {2}, Average 8% used = {3},
Average Basic E-prots used = {10},
Average E-prots used = {4}, Average H-prots used = {5},
Average Books used = {11}, Average base items used = {12},
Estimated average cost = {6}
5th percentile = {7}, 50th percentile = {8},
95th percentile = {9}", avgec, avg3, avg5, avg8, avge, avgh, formatted_avgCost, formatted_pct5, formatted_pct50, formatted_pct95, avgbe, avgb, avgi, avgsc);
        }

        /*
        public void PrintReport()
        {
            var trials = RunSimulation();

            var stats = trials.GroupBy(x => new { x.Attempts, x.Used15Boosters, x.Used25Boosters, x.UsedEProts, x.UsedHProts })
                            .Select(x => new TrialSignature { Attempts = x.Key.Attempts, Used15Boosters = x.Key.Used15Boosters, Used25Boosters = x.Key.Used25Boosters, UsedEProts = x.Key.UsedEProts, UsedHProts = x.Key.UsedHProts, Frequency = x.Count() })
                            .OrderBy(x => x).ToList();

            Console.WriteLine("Strategy: {0}, Target Level = {1}", Name, TargetLevel);

            int runningtotal = 0;

            foreach (var s in stats)
            {                
                runningtotal += s.Frequency;
                Console.WriteLine("{0} => {1}, {2}% ({3}% cumulative) B15 = {4}, B25 = {5}, EP = {6}, HP = {7}, WP Cost = {8}", s.Attempts, s.Frequency, (double)s.Frequency * 100.0 / trials.Count, (double)runningtotal * 100.0 / trials.Count,
                                                                                        s.Used15Boosters, s.Used25Boosters,
                                                                                        s.UsedEProts, s.UsedHProts,
                                                                                        CalculateCost(s.Used15Boosters, s.Used25Boosters, s.UsedEProts, s.UsedHProts));
            }

            Console.WriteLine("Min = {0}, Max = {1}, Average = {2}", trials.Min(x => x.Attempts), trials.Max(x => x.Attempts), trials.Average(x => x.Attempts));

            double avg15 = trials.Average(x => x.Used15Boosters);
            double avg25 = trials.Average(x => x.Used25Boosters);
            double avge = trials.Average(x => x.UsedEProts);
            double avgh = trials.Average(x => x.UsedHProts);

            Console.WriteLine("Average 15% used = {0}, Average 25% used = {1}", avg15, avg25);
            Console.WriteLine("Average E-prots used = {0}, Average H-prots used = {1}", avge, avgh);            
            Console.WriteLine("Estimated average cost = {0} WP", CalculateWPCost(avg15, avg25, avge, avgh));
        }
        */
    }
}
