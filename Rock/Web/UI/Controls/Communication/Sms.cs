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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock.Communication;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls.Communication
{
    /// <summary>
    /// SMS Communication Medium control
    /// </summary>
    public class Sms : MediumControl
    {
        #region UI Controls

        private RockDropDownList dvpFrom;
        private RockControlWrapper rcwMessage;
        private MergeFieldPicker mfpMessage;
        private Label lblCount;
        private RockTextBox tbMessage;

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // add the bootstrap-limit so that we can have a countdown of characters when entering SMS text
            int charLimit = this.CharacterLimit;
            if ( charLimit > 0 )
            {
                string script = $"$('#{tbMessage.ClientID}').limit({{maxChars: {charLimit}, counter:'#{lblCount.ClientID}', normalClass:'badge', warningClass:'badge-warning', overLimitClass: 'badge-danger'}});";
                ScriptManager.RegisterStartupScript( this, this.GetType(), $"limit-{this.ClientID}", script, true );
            }
        }

        #endregion Base Control Methods

        #region Properties

        /// <summary>
        /// Gets or sets the character limit.
        /// </summary>
        /// <value>
        /// The character limit.
        /// </value>
        public int CharacterLimit { get; set; }

        /// <summary>
        /// Gets or sets the selected numbers to display.
        /// </summary>
        /// <value>
        /// A guid list of numbers from System Phone Numbers to filter the dropdown list down.
        /// </value>
        public List<Guid> SelectedNumbers { get; set; }

        /// <summary>
        /// Sets control values from a communication record.
        /// </summary>
        /// <param name="communication">The communication.</param>
        public override void SetFromCommunication( CommunicationDetails communication )
        {
            var currentPerson = ( HttpContext.Current.CurrentHandler as RockPage )?.CurrentPerson;

            EnsureChildControls();

            var smsFromSystemPhoneNumberId = communication.SmsFromSystemPhoneNumberId;

#pragma warning disable CS0618 // Type or member is obsolete
            // Check the legacy defined value setting.
            if ( !smsFromSystemPhoneNumberId.HasValue && communication.SMSFromDefinedValueId.HasValue )
            {
                var fromDefinedValue = DefinedValueCache.Get( communication.SMSFromDefinedValueId.Value );

                if ( fromDefinedValue != null )
                {
                    smsFromSystemPhoneNumberId = SystemPhoneNumberCache.Get( fromDefinedValue.Guid )?.Id;
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete

            var valueItem = dvpFrom.Items.FindByValue( smsFromSystemPhoneNumberId.ToString() );
            if ( valueItem == null && smsFromSystemPhoneNumberId.HasValue )
            {
                var lookupSystemPhoneNumber = SystemPhoneNumberCache.Get( smsFromSystemPhoneNumberId.Value );
                if ( lookupSystemPhoneNumber != null && lookupSystemPhoneNumber.IsAuthorized( Rock.Security.Authorization.VIEW, currentPerson ) )
                {
                    dvpFrom.Items.Add( new ListItem( lookupSystemPhoneNumber.Name, lookupSystemPhoneNumber.Id.ToString() ) );
                }
            }

            dvpFrom.SetValue( smsFromSystemPhoneNumberId );
            tbMessage.Text = communication.SMSMessage;
        }

        /// <summary>
        /// Updates the a communication record from control values.
        /// </summary>
        /// <param name="communication">The communication.</param>
        public override void UpdateCommunication( CommunicationDetails communication )
        {
            EnsureChildControls();

            communication.SmsFromSystemPhoneNumberId = dvpFrom.SelectedValueAsId();
            communication.SMSMessage = tbMessage.Text;

#pragma warning disable CS0618 // Type or member is obsolete
            // Update the legacy values.
            communication.SMSFromDefinedValueId = null;
            if ( communication.SmsFromSystemPhoneNumberId.HasValue )
            {
                var systemPhoneNumber = SystemPhoneNumberCache.Get( communication.SmsFromSystemPhoneNumberId.Value );

                if ( systemPhoneNumber != null )
                {
                    communication.SMSFromDefinedValueId = DefinedValueCache.Get( systemPhoneNumber.Guid )?.Id;
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        #endregion

        #region CompositeControl Methods

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Clear();

            var selectedNumberGuids = SelectedNumbers; //GetAttributeValue( "FilterCategories" ).SplitDelimitedValues( true ).AsGuidList();
            var smsNumbers = SystemPhoneNumberCache.All()
                .OrderBy( spn => spn.Order )
                .ThenBy( spn => spn.Name )
                .ThenBy( spn => spn.Id )
                .ToList();

            if ( selectedNumberGuids.Any() )
            {
                smsNumbers = smsNumbers.Where( spn => selectedNumberGuids.Contains( spn.Guid ) ).ToList();
            }

            dvpFrom = new RockDropDownList();
            dvpFrom.SelectedIndex = -1;
            dvpFrom.DataSource = smsNumbers.Select( spn => new
            {
                spn.Name,
                spn.Id
            } );
            dvpFrom.DataTextField = "Name";
            dvpFrom.DataValueField = "Id";
            dvpFrom.DataBind();

            dvpFrom.ID = string.Format( "dvpFrom_{0}", this.ID );
            dvpFrom.Label = "From";
            dvpFrom.Help = "The number to originate message from (configured under Admin Tools > Communications > System Phone Numbers).";
            dvpFrom.Required = true;
            Controls.Add( dvpFrom );

            rcwMessage = new RockControlWrapper();
            rcwMessage.ID = string.Format( "rcwMessage_{0}", this.ID );
            rcwMessage.Label = "Message";
            rcwMessage.Help = "<span class='tip tip-lava'></span>";
            Controls.Add( rcwMessage );

            mfpMessage = new MergeFieldPicker();
            mfpMessage.ID = string.Format( "mfpMergeFields_{0}", this.ID );
            mfpMessage.MergeFields.Clear();
            mfpMessage.MergeFields.Add( "GlobalAttribute" );
            mfpMessage.MergeFields.Add( "Rock.Model.Person" );
            mfpMessage.CssClass += " margin-b-sm pull-right";
            mfpMessage.SelectItem += mfpMergeFields_SelectItem;
            rcwMessage.Controls.Add( mfpMessage );

            lblCount = new Label();
            lblCount.CssClass = "badge margin-all-sm pull-right";
            lblCount.ID = string.Format( "lblCount_{0}", this.ID );
            lblCount.Visible = this.CharacterLimit > 0;
            rcwMessage.Controls.Add( lblCount );

            tbMessage = new RockTextBox();
            tbMessage.ID = string.Format( "tbTextMessage_{0}", this.ID );
            tbMessage.TextMode = TextBoxMode.MultiLine;
            tbMessage.Rows = 3;
            tbMessage.Required = true;
            rcwMessage.Controls.Add( tbMessage );
        }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>
        /// The validation group.
        /// </value>
        public override string ValidationGroup
        {
            get
            {
                EnsureChildControls();
                return tbMessage.ValidationGroup;
            }
            set
            {
                EnsureChildControls();
                dvpFrom.ValidationGroup = value;
                mfpMessage.ValidationGroup = value;
                tbMessage.ValidationGroup = value;
            }
        }

        /// <summary>
        /// On new communication, initializes controls from sender values
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void InitializeFromSender( Person sender )
        {
            foreach ( var number in SystemPhoneNumberCache.All() )
            {
                var personAliasId = number.AssignedToPersonAliasId;
                if ( personAliasId.HasValue && sender.Aliases.Any( a => a.Id == personAliasId.Value ) )
                {
                    dvpFrom.SetValue( number.Id );
                    break;
                }
            }
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            dvpFrom.RenderControl( writer );
            rcwMessage.RenderControl( writer );
        }

        #endregion

        #region Events

        void mfpMergeFields_SelectItem( object sender, EventArgs e )
        {
            EnsureChildControls();
            tbMessage.Text += mfpMessage.SelectedMergeField;
            mfpMessage.SetValue( string.Empty );
        }

        #endregion
    }

}
