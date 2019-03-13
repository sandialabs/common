using gov.sandia.sld.common.db;
using Nancy;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.TinyIoc;
using Nancy.Diagnostics;
using Nancy.Bootstrapper;
using System.Diagnostics;

namespace COMMONWeb
{
    public class COMMONDatabaseBootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new DiagnosticsConfiguration() { Password = @"abc" };
            }
        }

        public COMMONDatabaseBootstrapper()
            : base()
        {
            JsonSettings.MaxJsonLength = int.MaxValue;
            m_db = new Database();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("app"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("content"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("languages"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("lib"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("vendor"));
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<IDataStore>(m_db);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            DiagnosticsHook.Disable(pipelines);

            //StaticConfiguration.EnableRequestTracing = true;
            StaticConfiguration.Caching.EnableRuntimeViewDiscovery = true;
            StaticConfiguration.Caching.EnableRuntimeViewUpdates = true;

#if DEBUG
            //pipelines.BeforeRequest.AddItemToStartOfPipeline(x =>
            //{
            //    Stopwatch watch = Stopwatch.StartNew();
            //    x.Items.Add("timer", watch);
            //    return null;
            //});

            //pipelines.AfterRequest.AddItemToEndOfPipeline(x =>
            //{
            //    if (!x.Items.ContainsKey("timer"))
            //        return;
            //    Stopwatch watch = (Stopwatch)x.Items["timer"];
            //    if (watch != null)
            //    {
            //        watch.Stop();
            //        //x.Trace.TraceLog.WriteLog(log => log.AppendLine(string.Format("Request took {0} ms", watch.ElapsedMilliseconds)));
            //        Trace.WriteLine($"Request {x.Request.Url.Path} took {watch.ElapsedMilliseconds} ms");
            //    }
            //});
#endif
        }

        private Database m_db;
    }
}
