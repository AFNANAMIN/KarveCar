﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarveDataServices.DataObjects;
using KarveDataServices.DataTransferObject;

namespace KarveDataServices.DataObjects
{
    public interface IReservationRequest: IValidDomainObject, IValueObject<ReservationRequestDto>, IHelperData
    {
    }
}
