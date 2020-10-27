﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Components
{
    interface IServiceProviderBuilder
    {
        IServiceProvider Build(IServiceCollection services);
    }
}
