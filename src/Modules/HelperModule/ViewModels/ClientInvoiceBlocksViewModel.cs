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
    class ClientInvoiceBlocksViewModel : GenericHelperViewModel<InvoiceBlockDto, BLOQUEFAC>
    {
      
        public ClientInvoiceBlocksViewModel(IDataServices dataServices, IRegionManager region, IEventManager manager, IDialogService dialogService) : base(string.Empty,dataServices, region, manager, dialogService)
        {
            GridIdentifier = KarveCommon.Generic.GridIdentifiers.HelperClientInvoice;
        }
        public override async Task<DataPayLoad> SetCode(DataPayLoad payLoad, IDataServices dataServices)
        {
            IHelperDataServices helperDal = DataServices.GetHelperDataServices();
            InvoiceBlockDto dto = payLoad.DataObject as InvoiceBlockDto;
            if (dto != null)
            {

                string codeId = await helperDal.GetUniqueId<BLOQUEFAC>(new BLOQUEFAC());
                dto.Code = codeId.Substring(0,3);
                payLoad.DataObject = dto;
            }
            return payLoad;
        }

    }
}
