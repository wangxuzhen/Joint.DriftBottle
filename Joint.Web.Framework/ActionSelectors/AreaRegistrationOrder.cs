using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Joint.Web.Framework
{
    //这个类设计的意义详见：http://wenku.baidu.com/link?url=heNatg7wB5IZJ5gvjseLavp2ldqcqOZiA4SzYig-yCq4GagfRZ1FcOoBXorcfzv9Q9FVnbl3IFZMIiPZO1GpIGywcbO6o90VTVBzRDcSqfe
    public abstract class AreaRegistrationOrder : AreaRegistration
    {
        protected static List<AreaRegistrationContext> areaContent = new List<AreaRegistrationContext>();
        protected static List<AreaRegistrationOrder> areaRegistration = new List<AreaRegistrationOrder>();

        protected AreaRegistrationOrder()
        {
        }

        private static void Register()
        {
            List<int[]> source = new List<int[]>();
            for (int i = 0; i < areaRegistration.Count; i++)
            {
                source.Add(new int[] { areaRegistration[i].Order, i });
            }
            foreach (int[] numArray in source.OrderBy(o => o[0]).ToList())
            {
                areaRegistration[numArray[1]].RegisterAreaOrder(areaContent[numArray[1]]);
            }
        }

        public static void RegisterAllAreasOrder()
        {
            AreaRegistration.RegisterAllAreas();
            Register();
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            areaContent.Add(context);
            areaRegistration.Add(this);
        }

        public abstract void RegisterAreaOrder(AreaRegistrationContext context);

        public abstract int Order { get; }
    }
}
