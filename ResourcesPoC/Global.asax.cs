using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ResourcesPoC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //We can manage the cultures we want to use base on our rules (FeatureTollgles ????)
        //The implementation will try to take the resources in the same order we add the cultures to this list (the first resource found will be used)
        //If no resourse is found in these culture, then it will be used the default culture.
        public static string[] CustomCultures = { "x-es-ES-SPIN", "x-es-ES-ADVENT" };


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //Setup Custom cultures fallback 
            RegisterCultures(CustomCultures);
            
        }


        protected void Application_BeginRequest()
        {
            //Use culture for the requests.
            CultureInfo ci = new CultureInfo(CustomCultures[0]);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private void configureCustomCulture(string newCultureName, CultureInfo parentCulture)
        {

            try
            {
                CultureAndRegionInfoBuilder cib = null;
                cib = new CultureAndRegionInfoBuilder(newCultureName, CultureAndRegionModifiers.None);
                cib.LoadDataFromCultureInfo(Thread.CurrentThread.CurrentCulture);

                if (parentCulture != null)
                    cib.Parent = parentCulture;

                cib.Register();

            }
            catch (Exception e)
            {

            }

        }

        private void UnregisterCultures(string[] cultures)
        {
            var alreadyRegisteredCultures = CultureInfo.GetCultures(CultureTypes.UserCustomCulture).ToList();
            foreach (var culture in cultures)
                if (alreadyRegisteredCultures.Exists( rc => rc.Name == culture.ToLower()))
                {
                    CultureAndRegionInfoBuilder.Unregister(culture.ToLower());
                }
        }

        private void RegisterCultures(string[] cultures)
        {
            UnregisterCultures(cultures);

            for (int i = CustomCultures.Length - 1; i >= 0; i--)
            {
                if (i == CustomCultures.Length - 1)
                {
                    configureCustomCulture(CustomCultures[i], null);
                }
                else
                {
                    configureCustomCulture(CustomCultures[i], new CultureInfo(CustomCultures[i + 1]));
                }
            }
        }
    }
}
