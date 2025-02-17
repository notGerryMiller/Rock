﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

using System.Collections.Generic;

namespace Rock.ViewModels.Blocks.Groups.GroupScheduleSignup
{
    /// <summary>
    /// A class representing the schedule data to pass down to mobile.
    /// </summary>
    public class PersonScheduleSignupBag
    {
        /// <summary>
        /// Gets or sets a string representing the group name.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="PersonScheduleSignupDataBag"/> to pass down to mobile.
        /// </summary>
        public List<PersonScheduleSignupDataBag> PersonScheduleSignups { get; set; }
    }
}
