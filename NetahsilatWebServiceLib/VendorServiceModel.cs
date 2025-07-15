using System;
using System.ServiceModel;
using NetahsilatWebServiceLib.VendorWebService;

namespace NetahsilatWebServiceLib
{
    public class VendorService
    {
        public VendorServiceConnectionResult ConnectVendorService(string serviceUrl, string userName, string password)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            EndpointAddress endpointAddress = new EndpointAddress(serviceUrl);

            var _client = new ChannelFactory<IVendorWebService>(binding, endpointAddress).CreateChannel();

            AuthenticationInfo authInfo = new AuthenticationInfo
            {
                UserName = userName,
                Password = password
            };

            return new VendorServiceConnectionResult
            {
                Client = _client,
                AuthInfo = authInfo
            };
        }

        public class VendorServiceConnectionResult
        {
            public IVendorWebService Client { get; set; }
            public AuthenticationInfo AuthInfo { get; set; }
        }
    }
} 