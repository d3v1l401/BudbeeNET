using BudbeeNet;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BudbeeCLI
{
    public static class Dumpers
    {
        public static string Info(this BudbeeNet.Order o)
        {
            return string.Format("Order ID: {0}\n\nReceiver Info\n{1}\n\nPhone number\n{2}\n\nAddress\n {3}, {4} {5}\n\n" +
                                 "Order Creation\n{6}\n\nDelivery Time Range\n{7} - {8}\n\n" +
                                 "Delivered: {9}\n",
                                    o.Token, o.Consumer.Name, o.Consumer.PhoneNumber, o.DeliveryAddress.Street, o.DeliveryAddress.PostalCode, o.DeliveryAddress.City,
                                    o.CreatedAt.ToString(), o.DeliveryWindow.Start.ToString(), o.DeliveryWindow.Stop.ToString(),
                                    o.ParcelSummary.Delivered > 0 ? "yes" : "no");
        }

        public static string DriverInfo(this BudbeeNet.Order od)
        {
            return string.Format("Driver ID: {0}\nName: {1}\nPhoto\n{2}\n",
                                    od.Driver.Id, od.Driver.Name, od.Driver.Icon);
        }

        public static string ProductInfo(this BudbeeNet.Order op)
        {
            return string.Format("Seller: {0}\nSeller Support:\n{1}\n{2}\n\nLogo\n{3}",
                                    op.Merchant.Name, op.Merchant.SupportEmail, op.Merchant.SupportPhone, op.Merchant.Logo);
        }

        public static string DriverCoordinates(this BudbeeNet.OrderDriver ob)
        {
            return string.Format("{ {0}, {1} } [ {2}, {3} km/h, {4} ]\n",
                                    ob.Coordinate.Longitude, ob.Coordinate.Latitude, ob.Heading, ob.Speed, ob.Accuracy);
        }
    }

    class Program
    {
        public class Parameters
        {
            [Option('o', "order", Required = true, HelpText = "The delivery (order) identifier given, use comma for more than one.")]
            public string OrderID { get; set; }

            [Option('b', "with-product", Required = false, HelpText = "Show the product information of your deliver (producer)")]
            public bool WithProduct { get; set; }

            [Option('t', "with-tracking", Required = false, HelpText = "Will print coordinates and speed of the driver, in a loop, so only one delivery ID.")]
            public bool WithTracking { get; set; }
        }

        static void Main(string[] args)
        {

            BudbeeNet.Client c = null;
            bool _printProduct = false;
            bool _printTracking = false;

            Parser.Default.ParseArguments<Parameters>(args)
                   .WithParsed<Parameters>(o =>
                   {
                       if (!string.IsNullOrWhiteSpace(o.OrderID))
                       {
                           _printProduct = o.WithProduct;
                           _printTracking = o.WithTracking;

                           if (o.OrderID.Contains(","))
                           {
                               // Multiple tracking codes
                               var _orderIds = o.OrderID.Split(',');

                               c = new BudbeeNet.Client(_orderIds);

                               _printTracking = false;
                           } else {
                               // Single one
                               c = new BudbeeNet.Client(o.OrderID);
                           }
                       }
                   });

            if (c == null)
                return;

            foreach (var _order in c.GetOrdersStatus())
            {
                Console.WriteLine("==============================\n");

                Console.WriteLine(_order.Info());

                if (_printProduct)
                    Console.WriteLine(_order.ProductInfo());

                if (_printTracking)
                {
                    Console.WriteLine(_order.DriverInfo());
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine(_order.DriverPosition().DriverCoordinates());
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("404"))
                            {
                                Console.WriteLine("No cached data for the driver, access to this data expired.");
                                break;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }

                Console.WriteLine("\n");
            }

        }
    }
}
