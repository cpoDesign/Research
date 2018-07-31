using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var list = new List<WorkingModel>()
            {
                new WorkingModel { Id = 1, Name = "Testing 1" },
                new WorkingModel { Id = 2, Name = "Testing 2" },
                new WorkingModel { Id = 3, Name = "Testing 3" }
            };

            foreach (var item in list)
            {
                string urlAction = item.GetType().GetAttributeValue((DisplayActionsAttribute da) => da.action);

                AttributeExtensions.WritePK(item);
            }
        }
    }


    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static void WritePK<T>(T item) where T : new()
        {
            // Just grabbing this to get hold of the type name:
            var type = item.GetType();

            // Get the PropertyInfo object:
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType() == typeof(KeyAttribute))
                    {
                        Console.WriteLine("The Primary Key for the {0} class is the {1} property with value: {2}", type.Name, property.Name, property.GetValue(item, null));
                    }
                }
            }
        }
    }
    public class GenericActions
    {
        internal const string Add = "Add";
        internal const string Delete = "Delete";
        internal const string Edit = "Edit";
    }

    [DisplayActions("/Controller/Action", new string[] { GenericActions.Delete, GenericActions.Add, GenericActions.Edit })]
    public class WorkingModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class DisplayActionsAttribute : Attribute
    {
        public string action { get; private set; }
        private string[] allowedActions;

        public DisplayActionsAttribute(string action, string[] allowedActions)
        {
            this.action = action;
            this.allowedActions = allowedActions;
        }
    }
}
