using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;

namespace Xervizio.Plugins.WebSwitch.Hosting {
    public class WebApiHost : IDisposable {
        private readonly string _baseAddress;

        public WebApiHost(string baseAddress) {
            _baseAddress = baseAddress;
        }

        public virtual void Start() {
            var config = new HttpSelfHostConfiguration(_baseAddress);

            //Route Catches the GET PUT DELETE typical REST based interactions (add more if needed)
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Put, HttpMethod.Delete) });

            //This allows POSTs to the RPC Style methods http://api/controller/action
            config.Routes.MapHttpRoute("API RPC Style", "api/{controller}/{action}",
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Finally this allows POST to typeical REST post address http://api/controller/
            config.Routes.MapHttpRoute("API Default 2", "api/{controller}/{action}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
                        
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
