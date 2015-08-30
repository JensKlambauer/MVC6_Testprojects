using System;
using System.Linq;

namespace MVC6MusicStore.Core.DAL.ADO.NET
{
    public static class SqlCommandExtensions
    {
        public static ISqlCommand ToInstance(this Type type)
        {
            var ctor = type.GetConstructors().SingleOrDefault();
            if (ctor != null && !ctor.GetParameters().Any())
            {
                var createdActivator = ObjectCreator.GetActivator<ISqlCommand>(ctor);
                return createdActivator();
            }

            throw new NotSupportedException("The call supports only classes that have a default constructor and accept no parameters.");
        }
    }
}