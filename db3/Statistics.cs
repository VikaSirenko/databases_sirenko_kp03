using System;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace db3
{
    public static class Statistics
    {
        public static void GenereteImage(OrderRepository orderRepository, DateTime firstDay, DateTime lastDay, string savePath)
        {
            int a = firstDay.Year - lastDay.Year;
            if (a == 0)
            {
                GenerateImageGraphicsForOneYear(firstDay, orderRepository, savePath);
            }
            else
            {
                GenerateGraphicImagesForSeveralYears(firstDay, lastDay, orderRepository, savePath);
            }

        }

        private static void GenerateImageGraphicsForOneYear(DateTime firstDay, OrderRepository orderRepository, string savePath)
        {
            var plt = new ScottPlot.Plot(600, 400);
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            double[] xs = DataGen.Consecutive(months.Length);
            double[] orders = new double[months.Length];
            for (int i = 0; i < months.Length; i++)
            {
                DateTime startDay = new DateTime(firstDay.Year, i + 1, 1);
                DateTime endDay;
                if (i == 11)
                {
                    endDay = new DateTime(firstDay.Year + 1, 1, 1);
                }
                else
                {
                    endDay = new DateTime(firstDay.Year, i + 2, 1);
                }
                int ordersCount = orderRepository.FilterByDate(startDay, endDay).Count;
                orders[i] = ordersCount;
            }
            plt.PlotScatter(xs, orders, label: "Orders");
            plt.Legend();
            plt.Ticks(xTickRotation: 45);
            plt.XTicks(xs, months);
            plt.SaveFig(savePath);
        }

        private static void GenerateGraphicImagesForSeveralYears(DateTime firstDay, DateTime lastDay, OrderRepository orderRepository, string savePath)
        {
            var plt = new ScottPlot.Plot(600, 400);
            int yearsCount = lastDay.Year - firstDay.Year + 2;
            string[] years = new string[yearsCount];
            for (int i = 0; i < yearsCount; i++)
            {
                years[i] = (firstDay.Year + i).ToString();
            }

            double[] xs = DataGen.Consecutive(years.Length);
            double[] orders = new double[years.Length];
            List<Order> orderList = orderRepository.FilterByDate(firstDay, lastDay);
            for (int i = 0; i < years.Length; i++)
            {
                int ordersCount = default;
                if (orderList.Count != 0)
                {
                    for (int j = 0; j < orderList.Count; j++)
                    {
                        if (firstDay.Year + i == orderList[j].order_date.Year)
                        {
                            ordersCount++;
                        }

                    }
                }


                orders[i] = ordersCount;
            }

            plt.PlotScatter(xs, orders, label: "Orders");
            plt.Legend();
            plt.Ticks(xTickRotation: 45);
            plt.XTicks(xs, years);
            plt.SaveFig(savePath);

        }


        public static Dictionary<int, int> GetTopTenProducts(ProductRepository productRepository, PurchaseRepository purchaseRepository)
        {

            Dictionary<int, int> dict = new Dictionary<int, int>();
            List<Purchase> purchasesList = purchaseRepository.GetAllPurchase();
            foreach (Purchase p in purchasesList)
            {
                if (dict.ContainsKey((int)p.product_id))
                {
                    dict[(int)p.product_id]++;
                }
                else
                    dict[(int)p.product_id] = 1;

            }

            dict = dict.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            return dict;
        }


        public static void GenerateGraphicForTopProducts(Dictionary<int, int> dict, string saveFile, ProductRepository productRepository)
        {
            double[] counts = new double[dict.Count];
            string[] products = new string[dict.Count];
            int i = dict.Count;
            foreach (KeyValuePair<int, int> keyValue in dict)
            {
                products[i - 1] = productRepository.GetProductById(keyValue.Key).product_name;
                counts[i - 1] = keyValue.Value;
                i--;

            }

            var plt = new ScottPlot.Plot(1000, 1000);

            if (dict.Count > 10)
            {
                double[] tenCounts = new double[10];
                string[] tenProducts = new string[10];
                for (int j = 0; j < 10; j++)
                {
                    tenCounts[j] = counts[j];
                    tenProducts[j] = products[j];
                }
                double[] xs = DataGen.Consecutive(tenProducts.Length);
                plt.PlotScatter(xs, tenCounts, label: "Products");
                plt.Legend();
                plt.Ticks(xTickRotation: 0);
                plt.XTicks(xs, tenProducts);
                plt.SaveFig(saveFile);
            }
            else if (dict.Count > 0 && dict.Count <= 10)
            {
                double[] xs = DataGen.Consecutive(products.Length);
                plt.PlotScatter(xs, counts, label: "Products");
                plt.Legend();
                plt.Ticks(xTickRotation: 0);
                plt.XTicks(xs, products);
                plt.SaveFig(saveFile);
            }

        }

        
    }

    public class PredictionData
    {
        public float Price { get; set; }
        public float NumberOfPurchases { get; set; }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float NumberOfPurchases { get; set; }
    }




}