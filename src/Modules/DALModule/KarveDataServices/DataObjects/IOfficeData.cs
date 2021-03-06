﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarveDataServices.DataTransferObject;

namespace KarveDataServices.DataObjects
{
    /// <summary>
    ///  Value of the OfficeData.
    /// </summary>
    public interface IOfficeData : IHelperBase, IValidDomainObject, IValueObject<OfficeDtos>
    {
        
        /// <summary>
        ///   CurrenciesDto.
        /// </summary>
        IEnumerable<CurrenciesDto> CurrenciesDto { get ; set ; }

    }
}
