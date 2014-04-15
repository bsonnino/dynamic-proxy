using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***\r\n Begin program - flexible proxy\r\n");
            //IRepository<Customer> customerRepository = new Repository<Customer>();
            //IRepository<Customer> customerRepository = 
            //    new LoggerRepository<Customer>(new Repository<Customer>());
            IRepository<Customer> customerRepository = RepositoryFactory.Create<Customer>();
            var customer = new Customer
            {
                Id = 1,
                Name = "Customer 1",
                Address = "Address 1"
            };
            customerRepository.Add(customer);
            customerRepository.Update(customer);
            customerRepository.Delete(customer);
            customerRepository.GetAll();
            customerRepository.GetById(1);
            Console.WriteLine("\r\nEnd program - flexible proxy\r\n***");
            Console.ReadLine();
        }
    }

    public class RepositoryFactory
    {
        private static void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }
        public static IRepository<T> Create<T>()
        {
            var repository = new Repository<T>();
            var dynamicProxy = new DynamicProxy<IRepository<T>>(repository);
            dynamicProxy.BeforeExecute += (s, e) => Log(
               "Before executing '{0}'", e.MethodName);
            dynamicProxy.AfterExecute += (s, e) => Log(
               "After executing '{0}'", e.MethodName);
            dynamicProxy.ErrorExecuting += (s, e) => Log(
               "Error executing '{0}'", e.MethodName);
            dynamicProxy.Filter = m => !m.Name.StartsWith("Get");
            return dynamicProxy.GetTransparentProxy() as IRepository<T>;
        }
    }
}
