﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarveCommon.Services;

namespace MasterModule.Common
{
    /// <summary>
    /// Handler algorithm for the event.
    /// </summary>
    public interface IChangeHandler
    {
        /// <summary>
        /// Insert and notify the toolbar.
        /// </summary>
        /// <param name="payLoad">Payload to be inserted</param>
        /// <param name="evDictionary">Event dictionary to be inserted</param>
         void OnInsert(DataPayLoad payLoad, IDictionary<string,object> evDictionary);
        /// <summary>
        /// Update the value and notify the toolbar.
        /// </summary>
        /// <param name="payLoad">Payload to be updated</param>
        /// <param name="evDictionary">Event dictionary to be updated.</param>
         void OnUpdate(DataPayLoad payLoad, IDictionary<string, object> evDictionary);
       
    }
}
