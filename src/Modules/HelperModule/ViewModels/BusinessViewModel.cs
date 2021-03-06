﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataObjects;
using KarveCommon.Services;
using KarveCommonInterfaces;
using KarveDataServices;
using KarveDataServices.DataTransferObject;
using Prism.Regions;

namespace HelperModule.ViewModels
{
    /// <summary>
    ///  BusinessViewModel. Model that maps the business helper.
    /// </summary>
    class BusinessViewModel: GenericHelperViewModel<BusinessDto, NEGOCIO>
    {
        public BusinessViewModel(IDataServices dataServices, IRegionManager region, IEventManager manager, IDialogService service) : base(String.Empty, dataServices, region, manager, service)
        {
            GridIdentifier = KarveCommon.Generic.GridIdentifiers.HelperBusiness;
        }
        public override async Task<DataPayLoad> SetCode(DataPayLoad payLoad, IDataServices dataServices)
        {
            IHelperDataServices helperDal = DataServices.GetHelperDataServices();
            BusinessDto dto = payLoad.DataObject as BusinessDto;
            if (dto != null)
            {
                string codeId = await helperDal.GetMappedUniqueId<BusinessDto,NEGOCIO>(dto);
                dto.Code = codeId.Substring(0, 2);
                payLoad.DataObject = dto;
            }
            return payLoad;
        }

      
    }
}
