﻿using KarveDataServices.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarveDataServices.DataObjects
{
    public interface ICompanyData : IHelperData
    {
        CompanyDto Value {set; get;}
    }
}
