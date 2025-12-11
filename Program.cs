using System.Text;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

public class HelloWorld
{
    public static void Main(string[] args)
    {
        //setup DI
        var serviceCollection = new ServiceCollection();

        // Register 
        serviceCollection.AddKeyedTransient<IShippingCostStrategy, ShippingCostStrategyForUPS>(ShippingOptions.ups);
        serviceCollection.AddKeyedTransient<IShippingCostStrategy, ShippingCostStrategyForFedEX>(ShippingOptions.fedex);
        serviceCollection.AddKeyedTransient<IShippingCostStrategy, ShippingCostStrategyForPurulator>(ShippingOptions.purulator);

        // Build the provider
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        Console.WriteLine("--- DI Strategy Validation Test ---");

        Address dummyAddr = new Address();

        var selectedOption = ShippingOptions.ups;

        // ASK DI: Give me the strategy linked to the key 'ShippingOptions.ups'
        var upsStrategy = serviceProvider.GetRequiredKeyedService<IShippingCostStrategy>(selectedOption);
        //in case i don't have an enum to distinguish them 
        IEnumerable<IShippingCostStrategy> _allStrategies; //use foreach to choose

        Order validOrder = new Order(
            selectedOption, 
            dummyAddr, 
            dummyAddr, 
            upsStrategy // DI gave us the correct class
        );

        Console.WriteLine($"Valid Order Cost ({selectedOption}): {validOrder.cost}"); 

        var anotherOption = ShippingOptions.fedex;
        
        // We just change the key, and DI finds the right class automatically
        var fedExStrategy = serviceProvider.GetRequiredKeyedService<IShippingCostStrategy>(anotherOption);

        Order fedExOrder = new Order(
            anotherOption, 
            dummyAddr, 
            dummyAddr, 
            fedExStrategy
        );

        Console.WriteLine($"Valid Order Cost ({anotherOption}): {fedExOrder.cost}");
    }
  //shipping class -change method -change the cost-each ethod has it's own strategy for cost 
  //adress - shipping options -order(get )
  public class Address
{
    public string? ContactName { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}
    public enum ShippingOptions
    {
        ups,
        fedex,
        purulator
    }
    public class Order
    {
        public ShippingOptions _shippingMethod;
        public Address _destination;
        public Address _origin;
        public IShippingCostStrategy _costStrategy;
        public Order(ShippingOptions shippingMethod,Address destination,Address origin,IShippingCostStrategy costStrategy)
        {
            _shippingMethod=shippingMethod;
            _destination=destination;
            _origin=origin;
            _costStrategy=costStrategy;
        }
        public ShippingOptions ShippingOptions
        {
            get
            {
                return _shippingMethod;
            }
        }
        public Address Origin
        {
            get
            {
                return _origin;
            }
        }
        public Address Destination
        {
            get
            {
                return _destination;
            }
        }
        public Double cost
        {
            get
            {
                return _costStrategy.CalcShippingCost(this);
            }
        }
    }
    public interface IShippingCostStrategy
    {
        public double CalcShippingCost(Order order);
    }
    public class ShippingCostStrategyForUPS : IShippingCostStrategy
    {
        public double CalcShippingCost(Order order)
        {
            if (order.ShippingOptions != ShippingOptions.ups)
            {
                throw new InvalidOperationException("Strategy Mismatch: You selected UPS Strategy, but the Order is marked as " + order.ShippingOptions);
            }
            
            return 7.25;
        }
    }
    public class ShippingCostStrategyForFedEX : IShippingCostStrategy
    {
        public double CalcShippingCost(Order order)
        {
            if (order.ShippingOptions != ShippingOptions.fedex)
            {
                throw new InvalidOperationException("Strategy Mismatch: You selected FedEx Strategy, but the Order is marked as " + order.ShippingOptions);
            }

            return 9.25;
        }
    }

    public class ShippingCostStrategyForPurulator : IShippingCostStrategy
    {
        public double CalcShippingCost(Order order)
        {
            // CHECK
            if (order.ShippingOptions != ShippingOptions.purulator)
            {
                throw new InvalidOperationException("Strategy Mismatch: You selected Purulator Strategy, but the Order is marked as " + order.ShippingOptions);
            }

            return 5.00;
        }
    }
}



