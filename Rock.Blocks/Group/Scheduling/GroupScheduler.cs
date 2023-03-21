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
using System.Data.Entity;
using System.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Enums.Blocks.Group.Scheduling;
using Rock.Field.Types;
using Rock.Model;
using Rock.Security;
using Rock.ViewModels.Blocks.Group.Scheduling.GroupScheduler;
using Rock.ViewModels.Utility;

namespace Rock.Blocks.Group.Scheduling
{
    /// <summary>
    /// Allows group schedules for groups and locations to be managed by a scheduler.
    /// </summary>

    [DisplayName( "Group Scheduler" )]
    [Category( "Group Scheduling" )]
    [Description( "Allows group schedules for groups and locations to be managed by a scheduler." )]
    [IconCssClass( "fa fa-calendar-alt" )]

    #region Block Attributes

    [BooleanField( "Enable Alternate Group Individual Selection",
        Key = AttributeKey.EnableAlternateGroupIndividualSelection,
        Description = "Determines if individuals may be selected from alternate groups.",
        ControlType = BooleanFieldType.BooleanControlType.Checkbox,
        DefaultBooleanValue = false,
        Order = 0,
        IsRequired = false )]

    [BooleanField( "Enable Parent Group Individual Selection",
        Key = AttributeKey.EnableParentGroupIndividualSelection,
        Description = "Determines if individuals may be selected from parent groups.",
        ControlType = BooleanFieldType.BooleanControlType.Checkbox,
        DefaultBooleanValue = false,
        Order = 1,
        IsRequired = false )]

    [BooleanField( "Enable Data View Individual Selection",
        Key = AttributeKey.EnableDataViewIndividualSelection,
        Description = "Determines if individuals may be selected from data views.",
        ControlType = BooleanFieldType.BooleanControlType.Checkbox,
        DefaultBooleanValue = false,
        Order = 2,
        IsRequired = false )]

    #endregion

    [Rock.SystemGuid.EntityTypeGuid( "7ADCE833-A785-4A54-9805-7335809C5367" )]
    [Rock.SystemGuid.BlockTypeGuid( "511D8E2E-4AF3-48D8-88EF-2AB311CD47E0" )]
    public class GroupScheduler : RockObsidianBlockType
    {
        #region Keys

        private static class AttributeKey
        {
            public const string EnableAlternateGroupIndividualSelection = "EnableAlternateGroupIndividualSelection";
            public const string EnableParentGroupIndividualSelection = "EnableParentGroupIndividualSelection";
            public const string EnableDataViewIndividualSelection = "EnableDataViewIndividualSelection";
        }

        #endregion

        #region Fields

        private List<int> _groupIds;
        private List<int> _locationIds;
        private List<int> _scheduleIds;

        #endregion

        #region Properties

        public override string BlockFileUrl => $"{base.BlockFileUrl}.obs";

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using ( var rockContext = new RockContext() )
            {
                var box = new GroupSchedulerInitializationBox();

                SetBoxInitialState( box, rockContext );

                return box;
            }
        }

        /// <summary>
        /// Sets the initial state of the box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <param name="rockContext">The rock context.</param>
        private void SetBoxInitialState( GroupSchedulerInitializationBox box, RockContext rockContext )
        {
            var block = new BlockService( rockContext ).Get( this.BlockId );
            block.LoadAttributes( rockContext );

            var filters = GetFilters( rockContext );

            box.Filters = filters;
            box.ScheduleOccurrences = GetScheduleOccurrences( rockContext, filters );
            box.ResourceSettings = GetResourceSettings();
            box.CloneSettings = GetCloneSettings();
            box.SecurityGrantToken = GetSecurityGrantToken();
        }

        /// <summary>
        /// Gets the filters, overriding any defaults with user preferences.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <returns>The filters.</returns>
        private GroupSchedulerFiltersBag GetFilters( RockContext rockContext )
        {
            var filters = new GroupSchedulerFiltersBag();

            // TODO (JPH): Hook into user preferences to override defaults, once supported in Obsidian blocks.

            ValidateFilters( rockContext, filters );

            return filters;
        }

        /// <summary>
        /// Validates the filters, overriding any selections if necessary.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="filters">The filters that should be validated.</param>
        private void ValidateFilters( RockContext rockContext, GroupSchedulerFiltersBag filters )
        {
            ValidateDateRange( filters );
            GetAuthorizedGroups( rockContext, filters );
            GetLocationsAndSchedules( rockContext, filters );
        }

        /// <summary>
        /// Validates the date range and attempts to set the first and last "end of week" dates (as well as the friendly date range) on the provided filters object.
        /// <para>
        /// If either component of the date range is in the past, it will be overwritten with the current date.
        /// </para>
        /// </summary>
        /// <param name="filters">The filters whose date range should be validated.</param>
        private void ValidateDateRange( GroupSchedulerFiltersBag filters )
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            if ( filters.DateRange != null )
            {
                // Utility method expects this format: SlidingDateRangeType|Number|TimeUnitType|StartDate|EndDate
                var range = filters.DateRange;
                var lowerDateString = range.LowerDate?.ToString( "o" );
                var upperDateString = range.UpperDate?.ToString( "o" );
                var delimitedValues = $"{range.RangeType}|{range.TimeValue}|{range.TimeUnit}|{lowerDateString}|{upperDateString}";
                var dateRange = RockDateTimeHelper.CalculateDateRangeFromDelimitedValues( delimitedValues );
                startDate = dateRange.Start;
                endDate = dateRange.End;
            }

            DateTime? firstEndOfWeekDate = null;
            if ( startDate.HasValue )
            {
                if ( startDate.Value < RockDateTime.Today )
                {
                    startDate = RockDateTime.Today;
                }

                firstEndOfWeekDate = startDate.Value.EndOfWeek( RockDateTime.FirstDayOfWeek );
            }

            DateTime? lastEndOfWeekDate = null;
            if ( endDate.HasValue )
            {
                if ( endDate.Value < RockDateTime.Today )
                {
                    endDate = RockDateTime.Today;
                }

                lastEndOfWeekDate = endDate.Value.EndOfWeek( RockDateTime.FirstDayOfWeek );
            }

            var format = "M/d";
            string friendlyDateRange = null;

            if ( startDate.HasValue && endDate.HasValue )
            {
                // Make sure we have a date range that makes sense.
                if ( endDate < startDate )
                {
                    endDate = startDate;
                    lastEndOfWeekDate = firstEndOfWeekDate;
                }

                // This doesn't need to be precise; just need to determine if we should try to list all "end of week" dates or just a range.
                var numberOfWeeks = ( endDate.Value - startDate.Value ).TotalDays / 7;
                if ( numberOfWeeks > 6 )
                {
                    friendlyDateRange = $"{firstEndOfWeekDate.Value.ToString( format )} - {lastEndOfWeekDate.Value.ToString( format )}";
                }
                else
                {
                    var endOfWeekDates = new List<DateTime>();
                    var endOfWeekDate = firstEndOfWeekDate.Value;
                    while ( endOfWeekDate <= lastEndOfWeekDate.Value )
                    {
                        endOfWeekDates.Add( endOfWeekDate );
                        endOfWeekDate = endOfWeekDate.AddDays( 7 );
                    }

                    friendlyDateRange = string.Join( ", ", endOfWeekDates.Select( d => d.ToString( format ) ) );
                }
            }
            else if ( firstEndOfWeekDate.HasValue )
            {
                friendlyDateRange = $"Beginning on {firstEndOfWeekDate.Value.ToString( format )}";

                // We'll probably want to consider putting a limit on how many weeks into the future may be scheduled.
            }
            else if ( lastEndOfWeekDate.HasValue )
            {
                var currentEndOfWeekDate = RockDateTime.Now.EndOfWeek( RockDateTime.FirstDayOfWeek );
                if ( lastEndOfWeekDate.Value != currentEndOfWeekDate )
                {
                    friendlyDateRange = $"{currentEndOfWeekDate.ToString( format )} - {lastEndOfWeekDate.Value.ToString( format )}";
                }
                else
                {
                    friendlyDateRange = currentEndOfWeekDate.ToString( format );
                }
            }

            if ( filters.DateRange != null )
            {
                filters.DateRange.LowerDate = startDate;
                filters.DateRange.UpperDate = endDate;
            }

            filters.FirstEndOfWeekDate = firstEndOfWeekDate;
            filters.LastEndOfWeekDate = lastEndOfWeekDate;
            filters.FriendlyDateRange = friendlyDateRange;
        }

        /// <summary>
        /// Gets the authorized groups from those selected within the filters, ensuring the current person has EDIT or SCHEDULE permission.
        /// <para>
        /// The groups will be updated on the filters object to include only those that are authorized.
        /// </para>
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="filters">The filters whose groups should be loaded and validated.</param>
        private void GetAuthorizedGroups( RockContext rockContext, GroupSchedulerFiltersBag filters )
        {
            if ( filters.Groups?.Any() != true )
            {
                return;
            }

            var groupGuids = filters.Groups
                .Select( g => g.Value.AsGuidOrNull() )
                .Where( g => g.HasValue )
                .Select( g => g.Value )
                .ToList();

            if ( !groupGuids.Any() )
            {
                filters.Groups = null;
                return;
            }

            // Get the selected groups and preload ParentGroup, as it's needed for a proper Authorization check.
            var groups = new GroupService( rockContext )
                .GetByGuids( groupGuids )
                .Include( g => g.ParentGroup )
                .AsNoTracking()
                .Where( g =>
                    g.IsActive
                    && !g.IsArchived
                    && g.GroupType.IsSchedulingEnabled
                    && !g.DisableScheduling
                )
                .ToList();

            // Ensure the current user has the correct permission(s) to schedule the selected groups and update the filters if necessary.
            groups = groups
                .Where( g =>
                    g.IsAuthorized( Authorization.EDIT, this.RequestContext.CurrentPerson )
                    || g.IsAuthorized( Authorization.SCHEDULE, this.RequestContext.CurrentPerson )
                )
                .ToList();

            filters.Groups = groups
                .Select( g => new ListItemBag
                {
                    Value = g.Guid.ToString(),
                    Text = g.Name
                } )
                .ToList();

            // Set aside the final list of group IDs for later use when selecting locations, schedules and occurrences to be scheduled.
            _groupIds = groups
                .Select( g => g.Id )
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets the available and selected locations and schedules, based on the combined, currently-applied filters.
        /// <para>
        /// The locations and schedules will be updated on the filters object.
        /// </para>
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="filters">The filters whose locations and schedules should be loaded.</param>
        private void GetLocationsAndSchedules( RockContext rockContext, GroupSchedulerFiltersBag filters )
        {
            if ( _groupIds?.Any() != true )
            {
                filters.Locations = null;
                filters.Schedules = null;
                return;
            }

            // Get all locations and schedules initially, so we can load the "available" lists.
            var groupLocationSchedules = new GroupLocationService( rockContext )
                .Queryable()
                .AsNoTracking()
                .Where( gl =>
                    _groupIds.Contains( gl.GroupId )
                    && gl.Location.IsActive
                )
                .SelectMany( gl => gl.Schedules, ( gl, s ) => new
                {
                    gl.Group,
                    gl.Location,
                    Schedule = s
                } )
                .Where( gls =>
                    gls.Schedule.IsActive
                    && gls.Schedule.EffectiveStartDate.HasValue
                    && (
                        !gls.Schedule.EffectiveEndDate.HasValue
                        || gls.Schedule.EffectiveEndDate.Value >= RockDateTime.Today
                    )
                )
                .ToList();

            // Refine the available (and selected) locations by the currently-selected schedules and vice versa.
            var selectedLocationGuids = ( filters.Locations?.SelectedLocations ?? new List<ListItemBag>() )
                .Select( l => l.Value?.AsGuidOrNull() )
                .Where( g => g.HasValue )
                .Select( g => g.Value )
                .ToList();

            var selectedScheduleGuids = ( filters.Schedules?.SelectedSchedules ?? new List<ListItemBag>() )
                .Select( s => s.Value?.AsGuidOrNull() )
                .Where( g => g.HasValue )
                .Select( g => g.Value )
                .ToList();

            var availableLocations = groupLocationSchedules
                .Where( gls => !selectedScheduleGuids.Any() || selectedScheduleGuids.Contains( gls.Schedule.Guid ) )
                .GroupBy( gls => gls.Location.Id )
                .Select( grouping => new ListItemBag
                {
                    Value = grouping.FirstOrDefault()?.Location?.Guid.ToString(),
                    Text = grouping.FirstOrDefault()?.Location?.ToString( true )
                } )
                .ToList();

            var selectedLocations = availableLocations
                .Where( l => selectedLocationGuids.Any( selected => selected.ToString() == l.Value ) )
                .ToList();

            filters.Locations = new GroupSchedulerLocationsBag
            {
                AvailableLocations = availableLocations,
                SelectedLocations = selectedLocations
            };

            var availableSchedules = groupLocationSchedules
                .Where( gls => !selectedLocationGuids.Any() || selectedLocationGuids.Contains( gls.Location.Guid ) )
                .GroupBy( gls => gls.Schedule.Id )
                .Select( grouping => new ListItemBag
                {
                    Value = grouping.FirstOrDefault()?.Schedule?.Guid.ToString(),
                    Text = grouping.FirstOrDefault()?.Schedule?.ToString()
                } )
                .ToList();

            var selectedSchedules = availableSchedules
                .Where( s => selectedScheduleGuids.Any( selected => selected.ToString() == s.Value ) )
                .ToList();

            filters.Schedules = new GroupSchedulerSchedulesBag
            {
                AvailableSchedules = availableSchedules,
                SelectedSchedules = selectedSchedules
            };
        }

        /// <summary>
        /// Gets the list of [group, location, schedule] occurrences, based on the currently-applied filters.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="filters">The currently-applied filters.</param>
        /// <returns>The list of [group, location, schedule] occurrences.</returns>
        private List<GroupSchedulerOccurrenceBag> GetScheduleOccurrences( RockContext rockContext, GroupSchedulerFiltersBag filters )
        {
            return null;
        }

        /// <summary>
        /// Gets the resource settings, overriding any defaults with user preferences.
        /// </summary>
        /// <returns>The resource settings.</returns>
        private GroupSchedulerResourceSettingsBag GetResourceSettings()
        {
            var enabledResourceListSourceTypes = GetEnabledResourceListSourceTypes();

            // TODO (JPH): Hook into user preferences to override defaults, once supported in Obsidian blocks.

            return new GroupSchedulerResourceSettingsBag
            {
                EnabledResourceListSourceTypes = enabledResourceListSourceTypes,
                SourceType = enabledResourceListSourceTypes.FirstOrDefault(),
                MatchType = default
            };
        }

        /// <summary>
        /// Gets the enabled resource list source types from which individuals may be scheduled.
        /// </summary>
        /// <returns>The enabled resource list source types.</returns>
        private List<ResourceListSourceType> GetEnabledResourceListSourceTypes()
        {
            var enabledTypes = new List<ResourceListSourceType> { ResourceListSourceType.Group };

            if ( GetAttributeValue( AttributeKey.EnableAlternateGroupIndividualSelection ).AsBoolean() )
            {
                enabledTypes.Add( ResourceListSourceType.AlternateGroup );
            }

            if ( GetAttributeValue( AttributeKey.EnableParentGroupIndividualSelection ).AsBoolean() )
            {
                enabledTypes.Add( ResourceListSourceType.ParentGroup );
            }

            if ( GetAttributeValue( AttributeKey.EnableDataViewIndividualSelection ).AsBoolean() )
            {
                enabledTypes.Add( ResourceListSourceType.DataView );
            }

            return enabledTypes;
        }

        /// <summary>
        /// Gets the clone settings, overriding any defaults with user preferences.
        /// </summary>
        /// <returns>The clone settings.</returns>
        private GroupSchedulerCloneSettingsBag GetCloneSettings()
        {
            // TODO (JPH): Hook into user preferences to override defaults, once supported in Obsidian blocks.

            return new GroupSchedulerCloneSettingsBag();
        }

        /// <summary>
        /// Gets the security grant token that will be used by UI controls on this block to ensure they have the proper permissions.
        /// </summary>
        /// <returns>A string that represents the security grant token.</returns>
        private string GetSecurityGrantToken()
        {
            return new Rock.Security.SecurityGrant().ToToken();
        }

        #endregion

        #region Block Actions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bag"></param>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult UpdateFilters( GroupSchedulerFiltersBag bag )
        {
            using ( var rockContext = new RockContext() )
            {
                ValidateFilters( rockContext, bag );

                return ActionOk( bag );
            }
        }

        /// <summary>
        /// Validates and applies the provided filters, then returns the new list of [group, location, schedule] occurrences, based on the applied filters.
        /// </summary>
        /// <param name="bag">The filters to apply.</param>
        /// <returns>An object containing the validated filters and new list of filtered [group, location, schedule] occurrences.</returns>
        [BlockAction]
        public BlockActionResult ApplyFilters( GroupSchedulerFiltersBag bag )
        {
            using ( var rockContext = new RockContext() )
            {
                ValidateFilters( rockContext, bag );

                var results = new GroupSchedulerAppliedFiltersBag
                {
                    AppliedFilters = bag,
                    ScheduleOccurrences = GetScheduleOccurrences( rockContext, bag )
                };

                return ActionOk( results );
            }
        }

        #endregion
    }
}
