﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 19/05/2021 15:28:43
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace mainModel
{
    [Serializable()]
    public partial class Favorite {

        public Favorite()
        {
            this.Cities = new List<City>();
            OnCreated();
        }

        public virtual long Id { get; set; }

        public virtual long? CityId { get; set; }

        public virtual IList<City> Cities { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}