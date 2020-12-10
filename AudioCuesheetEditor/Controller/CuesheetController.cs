﻿//This file is part of AudioCuesheetEditor.

//AudioCuesheetEditor is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//AudioCuesheetEditor is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see
//<http: //www.gnu.org/licenses />.
using AudioCuesheetEditor.Model.AudioCuesheet;
using AudioCuesheetEditor.Shared.ResourceFiles;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCuesheetEditor.Controller
{
    public class CuesheetController
    {
        private readonly IStringLocalizer<Localization> _localizer;

        private Cuesheet cuesheet;

        public CuesheetController(IStringLocalizer<Localization> localizer)
        {
            _localizer = localizer;
        }

        public LocalizedString GetLocalizedString(String key)
        {
            return _localizer[key];
        }

        public Cuesheet Cuesheet
        {
            get 
            { 
                if (cuesheet == null)
                {
                    cuesheet = new Cuesheet(this);
                }
                return cuesheet; 
            }
        }

        public Cuesheet NewCuesheet()
        {
            cuesheet = new Cuesheet(this);
            return Cuesheet;
        }

        public Track NewTrack()
        {
            return new Track(this);
        }

        public uint GetNextFreePosition()
        {
            return Cuesheet.NextFreePosition;
        }
    }
}