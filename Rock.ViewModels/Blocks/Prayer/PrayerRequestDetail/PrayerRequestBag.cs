// <copyright>
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

using System;

using Rock.ViewModels.Utility;

namespace Rock.ViewModels.Blocks.Prayer.PrayerRequestDetail
{
    public class PrayerRequestBag : EntityBagBase
    {
        /// <summary>
        /// Gets or sets a flag indicating  whether or not comments can be made against the request.
        /// </summary>
        public bool? AllowComments { get; set; }

        /// <summary>
        /// Gets or sets a description of the way that God has answered the prayer.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets the approved by person alias.
        /// </summary>
        public ListItemBag ApprovedByPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the PersonId of the Rock.Model.Person who approved this prayer request.
        /// </summary>
        public int? ApprovedByPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the date this prayer request was approved.
        /// </summary>
        public DateTime? ApprovedOnDateTime { get; set; }

        /// <summary>
        /// Gets or sets the campus.
        /// </summary>
        public ListItemBag Campus { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier.
        /// </summary>
        public int? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the Rock.Model.Category that this prayer request belongs to.
        /// </summary>
        public ListItemBag Category { get; set; }

        /// <summary>
        /// Gets or sets the CategoryId of the Rock.Model.Category that the PrayerRequest belongs to.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the created by person alias.
        /// </summary>
        public ListItemBag CreatedByPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the created by person alias identifier.
        /// </summary>
        public int? CreatedByPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the email address of the person requesting prayer.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the date that this prayer request was entered.
        /// </summary>
        public DateTime EnteredDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date that the prayer request expires. 
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the First Name of the person that this prayer request is about. This property is required.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the number of times this request has been flagged.
        /// </summary>
        public int? FlagCount { get; set; }

        /// <summary>
        /// Gets or sets an optional Guid foreign identifier.  This can be used for importing or syncing data to a foreign system
        /// </summary>
        public Guid? ForeignGuid { get; set; }

        /// <summary>
        /// Gets or sets an optional int foreign identifier.  This can be used for importing or syncing data to a foreign system
        /// </summary>
        public int? ForeignId { get; set; }

        /// <summary>
        /// Gets or sets an optional string foreign identifier.  This can be used for importing or syncing data to a foreign system
        /// </summary>
        public string ForeignKey { get; set; }

        /// <summary>
        /// TODO: GET CONFIRMATION AND DOCUMENT -CSF
        /// Gets or sets the group.
        /// </summary>
        public ListItemBag Group { get; set; }

        /// <summary>
        /// TODO: GET CLARIFICATION AND DOCUMENT
        /// Gets or sets the group id.
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this prayer request is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the prayer request has been approved. 
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether or not the request is public.
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this is an urgent prayer request.
        /// </summary>
        public bool? IsUrgent { get; set; }

        /// <summary>
        /// Gets or sets the Language Rock.Model.DefinedValue for this prayer request.
        /// </summary>
        public ListItemBag LanguageValue { get; set; }

        /// <summary>
        /// Gets or sets the DefinedValueId of the Rock.Model.DefinedValue that represents the Language for this prayer request.
        /// </summary>
        public int? LanguageValueId { get; set; }

        /// <summary>
        /// Gets or sets the Last Name of the person that this prayer request is about. This property is required.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the modified by person alias.
        /// </summary>
        public ListItemBag ModifiedByPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the modified by person alias identifier.
        /// </summary>
        public int? ModifiedByPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the modified date time.
        /// </summary>
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the number of times that this prayer request has been prayed for.
        /// </summary>
        public int? PrayerCount { get; set; }

        /// <summary>
        /// Gets or sets the requested by person alias.
        /// </summary>
        public ListItemBag RequestedByPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the PersonId of the Rock.Model.Person who is submitting the PrayerRequest
        /// </summary>
        public int? RequestedByPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the text/content of the request.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the FullName as it needs to be sent to the frontend
        /// </summary>
        public string FullName { get; set; }
    }
}
