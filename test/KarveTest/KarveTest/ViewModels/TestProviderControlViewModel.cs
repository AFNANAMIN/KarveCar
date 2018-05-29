﻿using System;
using System.Collections.Generic;
using System.Linq;
using KarveCommon.Generic;
using KarveCommon.Services;
using KarveDataServices;
using KarveDataServices.DataTransferObject;
using MasterModule.ViewModels;
using MasterModule.Views;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using Prism.Regions;
using ToolBarModule;
using KarveCommonInterfaces;

namespace KarveTest.ViewModels
{
    /// <summary>
    ///  TestProviderControlViewModel.
    /// </summary>
    [TestFixture]
    public class TestProviderControlViewModel
    {
        private Mock<IEventManager> _eventManager = new Mock<IEventManager>();
        private Mock<IDataServices> _dataServices = new Mock<IDataServices>();
        private Mock<IConfigurationService> _configurationService = new Mock<IConfigurationService>();
        private Mock<IUnityContainer> _unity = new Mock<IUnityContainer>();
        private  Mock<IRegionManager> _regionManager = new Mock<IRegionManager>();
        private Mock<ISupplierDataServices> _supplierMock = new Mock<ISupplierDataServices>();
        private Mock<IDialogService> _dialogService = new Mock<IDialogService>();
        private ProvidersControlViewModel _providersControlViewModel = null;
        
        [Test]
        public void Should_LoadSupplierSummary_Correctly()
        {
           
            List<SupplierSummaryDto> summary = new List<SupplierSummaryDto>()
            {
                new SupplierSummaryDto()
                {
                    Comercial = "KARVE1",
                    CP = "192029",
                    Direccion = "Via Biancamano",
                    Direccion2 = "Via RosaFiori",
                    AEAT = DateTime.Now,
                    Nombre = "Named",
                    Codigo = "0003",
                    Poblacion = "Barcelona",
                    Provincia = "Barcelona",
                    Telefono = "1920892"
                },
                new SupplierSummaryDto()
                {
                    Comercial = "KARVE2",
                    CP = "192029",
                    Direccion = "Via Biancamano",
                    Direccion2 = "Via RosaFiori",
                    AEAT = DateTime.Now,
                    Nombre = "Named",
                    Codigo = "0001",
                    Poblacion = "Barcelona",
                    Provincia = "Barcelona",
                    Telefono = "1920892"
                },
                new SupplierSummaryDto()
                {
                    Comercial = "KARVE3",
                    CP = "192029",
                    Direccion = "Via Biancamano",
                    Direccion2 = "Via RosaFiori",
                    AEAT = DateTime.Now,
                    Nombre = "Named",
                    Codigo = "0002",
                    Poblacion = "Barcelona",
                    Provincia = "Barcelona",
                    Telefono = "1920892"
                }
            };

            _providersControlViewModel = new ProvidersControlViewModel(_configurationService.Object, 
                                                                       _unity.Object, 
                                                                       _dataServices.Object, 
                                                                       _regionManager.Object, 
                                                                       _eventManager.Object);
            _supplierMock.Setup(c => c.GetSupplierAsyncSummaryDo()).ReturnsAsync(summary);
            _dataServices.Setup(ds => ds.GetSupplierDataServices()).Returns(_supplierMock.Object);
            _providersControlViewModel.StartAndNotify();
            IEnumerable<SupplierSummaryDto> collection = _providersControlViewModel.SummaryView as IEnumerable<SupplierSummaryDto>;
            Assert.GreaterOrEqual(collection.Distinct().Count(),1);
        }

        [Test]
        private void Should_SupplierNavigate_Correctly()
        {
            _providersControlViewModel = new ProvidersControlViewModel(_configurationService.Object,
                _unity.Object,
                _dataServices.Object,
                _regionManager.Object,
                _eventManager.Object);
            _providersControlViewModel.StartAndNotify();
            IEnumerable<SupplierSummaryDto> collection = _providersControlViewModel.SummaryView as IEnumerable<SupplierSummaryDto>;
            Assert.NotNull(collection);
            Assert.GreaterOrEqual(collection.Count(), 1);
            // from the view comes an openitem.
            SupplierSummaryDto dto = collection.FirstOrDefault();
            string tabName = dto.Codigo + "." + dto.Nombre;
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("supplierId", dto.Codigo);
            navigationParameters.Add(ScopedRegionNavigationContentLoader.DefaultViewName, tabName);
            var uri = new Uri(typeof(ProviderInfoView).FullName + navigationParameters, UriKind.Relative);
            _regionManager.Verify(manager => manager.RequestNavigate("TabRegion", tabName),Times.AtMostOnce);

            Assert.NotNull(collection);
            _providersControlViewModel.OpenItem.Execute(dto);
        }
    }
}
