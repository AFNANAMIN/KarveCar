﻿using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using AutoMapper;

namespace DataAccessLayer
{
    /// <summary>
    ///  Helper class for mapping inside the data abcess layer
    /// </summary>
    internal class MapperUtils
    {
        // 
        static public ObservableCollection<DataTransfer> GetMappedValue<T, DataTransfer>(T entity, IMapper mapper) where T : class
            where DataTransfer : class, new()
        {

            Contract.Requires(entity!=null, "Null entity");
            ObservableCollection<DataTransfer> transfer = new ObservableCollection<DataTransfer>();
            if (entity == null)
            {
                return transfer;
            }
            var value = mapper.Map<T, DataTransfer>(entity);
            transfer.Add(value);
            Contract.Ensures(transfer.Count > 0, "Count is not null");
            return transfer;
        }
    }
}
