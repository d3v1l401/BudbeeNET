using BudbeeNet.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudbeeNet
{
    public static class Ext
    {
        public static OrderBanner Banner(this Order o)
            => new Client(o.Token).GetOrdersBanner()[0];

        public static OrderDriver DriverPosition(this Order o)
            => new Client(o.Token).GetDriversPosition()[0];
    }

    public class Client
    {
        private List<string> m_orders = new List<string>();
        private Requester m_requester = new Requester();

        public Client(string _orderId)
        {
            if (string.IsNullOrWhiteSpace(_orderId))
                throw new Exception("Order identifier is empty");

            this.m_orders.Add(_orderId);
        }

        public Client(string[] _orderIds)
        {
            foreach (var _order in _orderIds)
            {
                if (string.IsNullOrWhiteSpace(_order))
                    throw new Exception("Order identifier is empty");

                this.m_orders.Add(_order);
            }
        }

        public Client(IList<string> _orderIds)
        {
            foreach (var _order in _orderIds)
            {
                if (string.IsNullOrWhiteSpace(_order))
                    throw new Exception("Order identifier is empty");

                this.m_orders.Add(_order);
            }
        }

        public static Order GetOrderStatus(string _orderId)
            => new Client(_orderId).GetOrdersStatus()[0];

        public static OrderDriver GetOrderDriver(string _orderId)
            => new Client(_orderId).GetDriversPosition()[0];

        public Order[] GetOrdersStatus()
        {
            var _orderStatuses = new Order[this.m_orders.Count];

            for (var i = 0; i < this.m_orders.Count; i++)
                _orderStatuses[i] = 
                        Order.FromJson(
                            Encoding.UTF8.GetString(
                                this.m_requester.Get("https://tracking.budbee.com/api/orders/" + this.m_orders[i], null)
                            )
                        );

            return _orderStatuses;
        }

        public OrderBanner[] GetOrdersBanner()
        {
            var _orderBanners = new OrderBanner[this.m_orders.Count];

            for (var i = 0; i < this.m_orders.Count; i++)
                _orderBanners[i] =
                    OrderBanner.FromJson(
                                Encoding.UTF8.GetString(this.m_requester.Get("https://tracking.budbee.com/api/v2/orders/" + this.m_orders[i] + "/banner", null))
                             );

            return _orderBanners;
        }

        public OrderDriver[] GetDriversPosition()
        {
            var _orderDrivers = new OrderDriver[this.m_orders.Count];

            var _status = this.GetOrdersStatus();
            for (var i = 0; i < this.m_orders.Count; i++)
                _orderDrivers[i] = OrderDriver.FromJson(Encoding.UTF8.GetString(this.m_requester.Get("https://tracking.budbee.com/api/positions/" + _status[i].Driver.Id, null)));

            return _orderDrivers;
        }

        public Dictionary<string, string> GetOrderType()
        {
            var _orderTypes = new Dictionary<string, string>();

            for (var i = 0; i < this.m_orders.Count; i++)
                _orderTypes.Add(this.m_orders[i], 
                    OrderType.FromJson(
                        Encoding.UTF8.GetString(this.m_requester.Get("https://tracking.budbee.com/api/orders/" + this.m_orders[i] + "/type", null))
                    ).Type
                );

            return _orderTypes;
        }
    }
}
