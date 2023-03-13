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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.ViewModels.Blocks;
using Rock.ViewModels.Blocks.Crm.PhotoOptOut;

namespace Rock.Blocks.Crm
{
    /// <summary>
    /// Allows a person to opt-out of future photo requests.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockObsidianDetailBlockType" />

    [DisplayName( "Photo Opt-Out" )]
    [Category( "CRM > PhotoRequest" )]
    [Description( "Allows a person to opt-out of future photo requests." )]
    [IconCssClass( "fa fa-question" )]

    #region Block Attributes

    #endregion

    [Rock.SystemGuid.EntityTypeGuid( "63f1d46a-eb78-4b0f-b398-099a83e058e8" )]
    [Rock.SystemGuid.BlockTypeGuid( "7e2dfb55-f1ab-4452-a5df-6ce65fbfddad" )]
    public class PhotoOptOutDetail : RockObsidianDetailBlockType
    {
        #region Keys

        private static class PageParameterKey
        {
            public const string Person = "Person";
        }

        private static class NavigationUrlKey
        {
            public const string ParentPage = "ParentPage";
        }

        #endregion Keys

        public override string BlockFileUrl => $"{base.BlockFileUrl}.obs";

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using ( var rockContext = new RockContext() )
            {
                var box = new DetailBlockBox<PhotoOptOutBag, PhotoOptOutDetailOptionsBag>();

                SetBoxInitialEntityState( box, rockContext );

                box.NavigationUrls = GetBoxNavigationUrls();

                return box;
            }
        }

        /// <summary>
        /// Sets the initial entity state of the box. Populates the Entity or
        /// ErrorMessage properties depending on the entity and permissions.
        /// </summary>
        /// <param name="box">The box to be populated.</param>
        /// <param name="rockContext">The rock context.</param>
        private void SetBoxInitialEntityState( DetailBlockBox<PhotoOptOutBag, PhotoOptOutDetailOptionsBag> box, RockContext rockContext )
        {
            Person entity = null;
            string personKey = PageParameter( PageParameterKey.Person );

            try
            {
                entity = new PersonService( rockContext ).GetByPersonActionIdentifier( personKey, "OptOut" );
                if ( entity == null )
                {
                    entity = new PersonService( rockContext ).GetByUrlEncodedKey( personKey );
                }
            }
            catch ( System.FormatException )
            {
                box.ErrorMessage = "No, that's not right. Are you sure you copied that web address correctly?";
                return;
            }
            catch ( Exception ex )
            {
                box.ErrorMessage = "No, that's not right. Are you sure you copied that web address correctly?";
                return;
            }

            if ( entity == null )
            {
                box.ErrorMessage = "That's odd. We could not find your record in our system. Please contact our office at the number below.";
                return;
            }

            if ( entity != null )
            {
                try
                {
                    GroupService service = new GroupService( rockContext );
                    Group photoRequestGroup = service.GetByGuid( Rock.SystemGuid.Group.GROUP_PHOTO_REQUEST.AsGuid() );
                    var groupMember = photoRequestGroup.Members.Where( m => m.PersonId == entity.Id ).FirstOrDefault();
                    if ( groupMember == null )
                    {
                        groupMember = new GroupMember();
                        groupMember.GroupId = photoRequestGroup.Id;
                        groupMember.PersonId = entity.Id;
                        groupMember.GroupRoleId = photoRequestGroup.GroupType.DefaultGroupRoleId ?? 0;
                        photoRequestGroup.Members.Add( groupMember );
                    }

                    groupMember.GroupMemberStatus = GroupMemberStatus.Inactive;

                    rockContext.SaveChanges();
                    box.Entity = new PhotoOptOutBag();
                    box.Entity.IsOptOutSuccessful = true;
                }
                catch ( Exception ex )
                {
                    box.ErrorMessage = "Something went wrong and we could not save your request. If it happens again please contact our office at the number below.";
                }
            }
            else
            {
                box.ErrorMessage = "That's odd. We could not find your record in our system. Please contact our office at the number below.";
            }
        }

        /// <summary>
        /// Gets the box navigation URLs required for the page to operate.
        /// </summary>
        /// <returns>A dictionary of key names and URL values.</returns>
        private Dictionary<string, string> GetBoxNavigationUrls()
        {
            return new Dictionary<string, string>
            {
                [NavigationUrlKey.ParentPage] = this.GetParentPageUrl()
            };
        }

        #endregion
    }
}
