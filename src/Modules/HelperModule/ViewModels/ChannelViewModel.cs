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
    class ChannelViewModel : GenericHelperViewModel<ChannelDto, CANAL>
    {
        public override async Task<DataPayLoad> SetCode(DataPayLoad payLoad, IDataServices dataServices)
        {
            IHelperDataServices helperDal = DataServices.GetHelperDataServices();
            var dto = payLoad.DataObject as ChannelDto;
            if (dto != null)
            {
                string codeId = await helperDal.GetMappedUniqueId<ChannelDto, CANAL>(dto);
                dto.Code = codeId.Substring(0, 2);
                payLoad.DataObject = dto;
            }
            return payLoad;
        }
        public ChannelViewModel(IDataServices dataServices, IRegionManager region, IEventManager manager, IDialogService service) : base(String.Empty, dataServices, region, manager, service)
        {
            GridIdentifier = KarveCommon.Generic.GridIdentifiers.HelperChannel;
        }

    
    }
}
