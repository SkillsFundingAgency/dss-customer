using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.ServiceTests
{
    [TestFixture]
    public class PatchCustomerHttpTriggerServiceTests
    {
        private IPatchCustomerHttpTriggerService _patchCustomerHttpTrigger;
        private ICustomerPatchService _customerPatchService;
        private IDocumentDBProvider _documentDbProvider;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;
        private string _json;
        private string _customerString;
        private Guid _customerId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");


        [SetUp]
        public void Setup()
        {
            _documentDbProvider = Substitute.For<IDocumentDBProvider>();
            _customerPatchService = Substitute.For<ICustomerPatchService>();
            _patchCustomerHttpTrigger = Substitute.For<PatchCustomerHttpTriggerService>(_customerPatchService, _documentDbProvider);
            _customer = Substitute.For<Models.Customer>();
            _customerPatch = Substitute.For<CustomerPatch>();
            _json = JsonConvert.SerializeObject(_customerPatch);
            _customerString = JsonConvert.SerializeObject(_customer);
            _customerPatchService.Patch(_json, _customerPatch).Returns(_customerString);
        }

        //[Test]
        //public void PatchCustomerHttpTriggerServiceTests_PatchResource_ReturnsNullWhenCustomerJsonIsNullOrEmpty()
        //{
        //     Act
        //    var result = _patchCustomerHttpTrigger.PatchResource(null, Arg.Any<CustomerPatch>());

        //     Assert
        //    Assert.IsNull(result);
        //}

        //[Test]
        //public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsNullWhenCustomerPatchServicePatchJsonIsNullOrEmpty()
        //{
        //    _customerPatchService.Patch(Arg.Any<string>(), Arg.Any<CustomerPatch>()).ReturnsNullForAnyArgs();

        //    // Act
        //    var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

        //    // Assert
        //    Assert.IsNull(result);
        //}

        //[Test]
        //public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsNullWhenResourceCannotBeUpdated()
        //{
        //    _documentDbProvider.UpdateCustomerAsync(_customerString, _customerId).ReturnsNull();

        //    // Act
        //    var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

        //    // Assert
        //    Assert.IsNull(result);
        //}

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsResourceWhenUpdated()
        {
            const string documentServiceResponseClass = "Microsoft.Azure.Documents.DocumentServiceResponse, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
            const string dictionaryNameValueCollectionClass = "Microsoft.Azure.Documents.Collections.DictionaryNameValueCollection, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

            var resourceResponse = new ResourceResponse<Document>(new Document());
            var documentServiceResponseType = Type.GetType(documentServiceResponseClass);

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var headers = new NameValueCollection { { "x-ms-request-charge", "0" } };

            var headersDictionaryType = Type.GetType(dictionaryNameValueCollectionClass);

            var headersDictionaryInstance = Activator.CreateInstance(headersDictionaryType, headers);

            var arguments = new[] { Stream.Null, headersDictionaryInstance, HttpStatusCode.OK, null };

            var documentServiceResponse = documentServiceResponseType.GetTypeInfo().GetConstructors(flags)[0].Invoke(arguments);

            var responseField = typeof(ResourceResponse<Document>).GetTypeInfo().GetField("response", flags);

            responseField?.SetValue(resourceResponse, documentServiceResponse);

            _documentDbProvider.UpdateCustomerAsync(_customerString, _customerId).Returns(Task.FromResult(resourceResponse).Result);

            // Act
            var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.Customer>(result);

        }

        //[Test]
        //public async Task PatchCustomerHttpTriggerServiceTests_GetCustomerByIdAsync_ReturnsNullWhenResourceHasNotBeenFound()
        //{
        //    _documentDbProvider.GetCustomerByIdForUpdateAsync(_customerId).ReturnsNull();

        //    // Act
        //    var result = await _patchCustomerHttpTrigger.GetCustomerByIdAsync(Arg.Any<Guid>());

        //    // Assert
        //    Assert.IsInstanceOf<string>(result);
        //    Assert.IsEmpty(result);
        //}

        //[Test]
        //public async Task PatchCustomerHttpTriggerServiceTests_GetCustomerByIdAsync_ReturnsResourceWhenResourceHasBeenFound()
        //{
        //    _documentDbProvider.GetCustomerByIdForUpdateAsync(_customerId).Returns(Task.FromResult(_json).Result);

        //    // Act
        //    var result = await _patchCustomerHttpTrigger.GetCustomerByIdAsync(_customerId);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<string>(result);
        //}

    }
}
