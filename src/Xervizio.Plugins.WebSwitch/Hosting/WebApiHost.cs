using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Xervizio.Plugins.WebSwitch.Hosting {
    public class WebApiHost : IDisposable {
        private readonly string _baseAddress;

        public WebApiHost(string baseAddress) {
            _baseAddress = baseAddress;
        }

        public virtual void Start() {
            var config = new HttpSelfHostConfiguration(_baseAddress);

            config.Routes.MapHttpRoute(
                name: "ActionApis",
                routeTemplate: "api/host/{action}",
                defaults: new { controller = "host", action = "DefaultAction"}
            );
            
            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "", id = RouteParameter.Optional }
            );



            var server =new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            _host = server;
        }

        public virtual void Stop() {            
            _host.Dispose();
            _host = null;
        }

        #region IDisposable

        public void Dispose() {
            Dispose(true);
        }

        public void Dispose(bool isDisposing) {
            if (isDisposing) {
                if (!_isDisposed) {
                    System.GC.SuppressFinalize(this);
                }
                _isDisposed = true;
            }
        }

        ~WebApiHost() {
            Dispose(_isDisposed);
        }

        private bool _isDisposed;
        private IDisposable _host;

        #endregion
    }
}
